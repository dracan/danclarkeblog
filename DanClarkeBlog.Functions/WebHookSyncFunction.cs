using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using Autofac;
using Microsoft.ServiceBus.Messaging;

namespace DanClarkeBlog.Functions
{
    public static class WebHookSyncFunction
    {
        [FunctionName("WebHookSyncFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, [ServiceBus("dropboxupdates", AccessRights.Listen)] IAsyncCollector<string> message)
        {
            var ct = CancellationToken.None;
            var container = FunctionBootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();
            var hashVerify = container.Resolve<IHashVerify>();
            var settings = container.Resolve<Settings>();

            try
            {
                // req.GetQueryNameValuePairs doesn't seem to exist in this version of the libraries - so use regex instead
                var challengeMatch = Regex.Match(req.RequestUri.Query, @".*[?&]challenge=(.*?)[&$]");

                if (challengeMatch.Success && challengeMatch.Groups.Count == 2)
                {
                    log.Info("Received Dropbox challenge request");

                    var challenge = challengeMatch.Groups[1].Value;

                    await message.AddAsync("", ct);

                    await notificationTarget.SendMessageAsync("Received a challenge request from Dropbox. Replying to accept.", ct);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(challenge, Encoding.UTF8, "text/plain")
                    };
                }

                var signature = req.Headers.GetValues("X-Dropbox-Signature").FirstOrDefault();
                var body = await req.Content.ReadAsStringAsync();

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(settings.DropboxAppSecret)))
                {
                    if (!hashVerify.VerifySha256Hash(hmac, body, signature))
                    {
                        await notificationTarget.SendMessageAsync("Webhook request from Dropbox failed HMAC check", ct);
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }

                await message.AddAsync("INCREMENTAL_DROPBOX_UPDATE", ct);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await notificationTarget.SendMessageAsync($"An exception occurred in the Dropbox webhook endpoint function: {ex}", ct);
                throw;
            }
        }
    }
}
