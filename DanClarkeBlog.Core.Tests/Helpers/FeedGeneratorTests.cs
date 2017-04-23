using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class FeedGeneratorTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task RssGeneratorTests()
        {
            var container = TestBootstrapper.Init();

            var settings = container.Resolve<Settings>();
            var sut = container.Resolve<IFeedGenerator>();

            var results = await sut.GenerateRssAsync(CancellationToken.None);

            Assert.NotNull(results);
            Assert.StartsWith("<?xml", results);
            Assert.Contains(settings.SiteHomeUri, results);
        }
    }
}
