using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using System.Text.RegularExpressions;
using System.Threading;
using Autofac;
using DanClarkeBlog.Core.Helpers;

namespace DanClarkeBlog.Functions.WebHookSync
{
    public class WebHookSyncFunction
    {
        public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log, out string message)
        {
            var ct = CancellationToken.None;
            var container = Bootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();

            try
            {
                //(todo) Verify Dropbox HMAC here (see X-Dropbox-Signature header)
                // Example in .NET here http://www.jerriepelser.com/blog/creating-a-dropbox-webhook-in-aspnet/

                // req.GetQueryNameValuePairs doesn't seem to exist in this version of the libraries - so use regex instead
                var challengeMatch = Regex.Match(req.RequestUri.Query, @".*[?&]challenge=(.*?)[&$]");

                if (challengeMatch.Success && challengeMatch.Groups.Count == 2)
                {
                    log.Info("Received Dropbox challenge request");

                    var challenge = challengeMatch.Groups[1].Value;

                    message = "";

                    notificationTarget.SendMessageAsync("Received a challenge request from Dropbox. Replying to accept.", ct);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                           {
                               Content = new StringContent(challenge, System.Text.Encoding.UTF8, "text/plain")
                           };
                }

                message = "INCREMENTAL_DROPBOX_UPDATE";

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                notificationTarget.SendMessageAsync($"An exception occurred in the Dropbox webhook endpoint function: {ex}", ct);
                throw;
            }
        }
    }
}
