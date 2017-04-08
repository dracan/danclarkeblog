using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.WebHookSync
{
    public class WebHookSyncFunction
    {
        /*
        // This works for the Dropbox challenge request
        var challenge = req.GetQueryNameValuePairs()
            .FirstOrDefault(q => string.Compare(q.Key, "challenge", true) == 0)
            .Value;

        var res = req.CreateResponse(HttpStatusCode.OK);
        res.Content = new StringContent(challenge, System.Text.Encoding.UTF8, "text/plain");
        return res;
        */

        public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log, out string message)
        {
            //(todo) Verify Dropbox HMAC here (see X-Dropbox-Signature header)
            // Example in .NET here http://www.jerriepelser.com/blog/creating-a-dropbox-webhook-in-aspnet/

            message = "OK";

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
