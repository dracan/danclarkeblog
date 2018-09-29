using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using DanClarkeBlog.Core.Helpers;
using Settings = DanClarkeBlog.Core.Settings;
using Serilog;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace DanClarkeBlog.Web.Controllers
{
    public class WebhookController : Controller
    {
        private readonly Settings _settings;
        private readonly INotificationTarget _notificationTarget;
        private readonly IHashVerify _hashVerify;
        private readonly IMessageQueue _messageQueue;

        public WebhookController(Settings settings, INotificationTarget notificationTarget, IHashVerify hashVerify, IMessageQueue messageQueue)
        {
            _settings = settings;
            _notificationTarget = notificationTarget;
            _hashVerify = hashVerify;
            _messageQueue = messageQueue;
        }

        [HttpGet]
        [Route("/webhook")]
        public async Task<ContentResult> Challenge([FromQuery] string challenge, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Received Dropbox challenge request");

                await _notificationTarget.SendMessageAsync("Received a challenge request from Dropbox. Replying to accept.", cancellationToken);

                return Content(challenge, "text/plain", System.Text.Encoding.UTF8);
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
                Log.Information("Received Dropbox change notification");

                HttpContext.Request.Headers.TryGetValue("X-Dropbox-Signature", out var signature);

                string body;

                using(var reader = new StreamReader(HttpContext.Request.Body))
                {
                    body = reader.ReadToEnd();
                }

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.DropboxAppSecret)))
                {
                    if (!_hashVerify.VerifySha256Hash(hmac, body, signature))
                    {
                        Log.Warning("Dropbox webhook notification failed hmac check");

                        await _notificationTarget.SendMessageAsync("Webhook request from Dropbox failed HMAC check", cancellationToken);
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }

                _messageQueue.Send("sync", JsonConvert.SerializeObject(new { IsIncremental = true }));

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
