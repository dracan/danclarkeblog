#r "Newtonsoft.Json"
#r "DanClarkeBlog.Core.dll"
#r "Microsoft.WindowsAzure.Storage"

using System;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using System.Configuration;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
    log.Info($"Starting Dropbox Sync ...");

    if(req.Method == HttpMethod.Get)
    {
        log.Info($"QUERY = {req.RequestUri.Query}");

        var res = new HttpResponseMessage(HttpStatusCode.OK);
        res.Content = new StringContent("Your response text");
        return res;

        // var challenge = req.GetQueryNameValuePairs()
        //     .FirstOrDefault(q => string.Compare(q.Key, "challenge", true) == 0)
        //     .Value;

        // return req.CreateResponse(HttpStatusCode.OK, new {
        //     challenge = challenge
        // });
    }

    // if(req.Method != HttpMethod.Post)
    // {
    //     return req.CreateResponse(HttpStatusCode.BadRequest, new {
    //         error = "Unhandled method"
    //     });
    // }

    // string jsonContent = await req.Content.ReadAsStringAsync();

    // log.Info(jsonContent);

    // // dynamic data = JsonConvert.DeserializeObject(jsonContent);

    // return req.CreateResponse(HttpStatusCode.OK, new {
    // });
}
