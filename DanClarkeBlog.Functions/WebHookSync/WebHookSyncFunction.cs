using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using System.Text.RegularExpressions;

namespace DanClarkeBlog.Functions.WebHookSync
{
    public class WebHookSyncFunction
    {
        public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log, out string message)
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

                return new HttpResponseMessage(HttpStatusCode.OK)
                          {
                              Content = new StringContent(challenge, System.Text.Encoding.UTF8, "text/plain")
                          };
            }

            message = "INCREMENTAL_DROPBOX_UPDATE";

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
