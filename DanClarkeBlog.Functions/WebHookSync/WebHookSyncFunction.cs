using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using Microsoft.Azure.WebJobs;

namespace DanClarkeBlog.Functions.WebHookSync
{
    public class WebHookSyncFunction
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> message)
        {
            var ct = CancellationToken.None;
            var container = FunctionBootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();
            var hashVerify = container.Resolve<IHashVerify>();
            var settings = container.Resolve<Settings>();

            try
            {
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
