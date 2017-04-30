using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class DropboxHelperIntegrationTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task ListFiles()
        {
            var httpClient = new HttpClientHelper();
            var container = TestBootstrapper.Init(httpClient);

            var sut = container.Resolve<IDropboxHelper>();

            var files = await sut.GetFilesAsync("", CancellationToken.None);

            Assert.NotEmpty(files);
        }
    }
}
