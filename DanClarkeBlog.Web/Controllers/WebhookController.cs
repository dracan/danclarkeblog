using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using DanClarkeBlog.Core.Helpers;
using Settings = DanClarkeBlog.Core.Settings;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Web.Controllers
{
    public class WebhookController : Controller
    {
        private readonly Settings _settings;
        private readonly INotificationTarget _notificationTarget;
        private readonly IHashVerify _hashVerify;
        private readonly IMessageQueue _messageQueue;
        private readonly ILogger _logger;

        public WebhookController(IOptions<Settings> settings,
                                 INotificationTarget notificationTarget,
                                 IHashVerify hashVerify,
                                 IMessageQueue messageQueue,
                                 ILogger<WebhookController> logger)
        {
            _settings = settings.Value;
            _notificationTarget = notificationTarget;
            _hashVerify = hashVerify;
            _messageQueue = messageQueue;
            _logger = logger;
        }

        [HttpGet]
        [Route("/webhook")]
        public async Task<ContentResult> Challenge([FromQuery] string challenge, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received Dropbox challenge request");

                await _notificationTarget.SendMessageAsync("Received a challenge request from Dropbox. Replying to accept.", cancellationToken);

                return Content(challenge, "text/plain", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                await _notificationTarget.SendMessageAsync($"An exception occurred in the Dropbox webhook endpoint function: {ex}", cancellationToken);
                throw;
            }
        }

        [HttpPost]
        [Route("/webhook")]
        public async Task<HttpResponseMessage> Notification(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received Dropbox change notification");

                HttpContext.Request.Headers.TryGetValue("X-Dropbox-Signature", out var signature);

                string body;

                using(var reader = new StreamReader(HttpContext.Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.DropboxAppSecret)))
                {
                    if (!_hashVerify.VerifySha256Hash(hmac, body, signature))
                    {
                        _logger.LogWarning("Dropbox webhook notification failed hmac check");

                        await _notificationTarget.SendMessageAsync("Webhook request from Dropbox failed HMAC check", cancellationToken);
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }

                await _messageQueue.SendAsync("sync", JsonConvert.SerializeObject(new { IsIncremental = true }), cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await _notificationTarget.SendMessageAsync($"An exception occurred in the Dropbox webhook endpoint function: {ex}", cancellationToken);
                throw;
            }
        }
    }
}
