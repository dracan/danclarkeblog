using System.Diagnostics;
using System.IO;
using System.Net.Http;
using DanClarkeBlog.Functions.WebHookSync;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;

namespace DanClarkeBlog.Functions.Tests
{
    public class WebHookSyncTests
    {
        [Fact, Trait("Category", "Unit")]
        public void Blah()
        {
            var req = new HttpRequestMessage();
            string message;

            var traceWriter = Substitute.For<TraceWriter>();

            WebHookSyncFunction.Run(req, traceWriter, out message);
        }
    }
}
