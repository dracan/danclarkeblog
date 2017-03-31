using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class DropboxHelperIntegrationTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task ListFiles()
        {
            var settings = new Settings { DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken") };
            var httpClient = new HttpClientHelper();

            var sut = new DropboxHelper(settings, httpClient);

            var files = await sut.GetFilesAsync("", CancellationToken.None);

            Assert.NotEmpty(files);
        }
    }
}
