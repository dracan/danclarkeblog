using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Dropbox;
using DanClarkeBlog.Core.Helpers;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Helpers
{
    public class DropboxHelperUnitTests
    {
        [Fact, Trait("Category", "Unit")]
        public async Task ListFilesInSingleRequest()
        {
            //(todo) These tests should be using the TestBootstrapper like the other tests.

            var settings = new Settings { DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken") };

            var httpClient = Substitute.For<IHttpClientHelper>();

            var dropboxResponse = new DropboxApiResponseListFiles
                                  {
                                      entries = new[]
                                                {
                                                    new Entry { name = "Item 1" }
                                                }
                                  };

            httpClient.PostAsync(new Uri("https://api.dropboxapi.com/2/files/list_folder"), @"{""path"":""""}", Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(JsonConvert.SerializeObject(dropboxResponse)));

            var sut = new DropboxHelper(settings, httpClient);

            var files = await sut.GetFilesAsync("", CancellationToken.None);

            Assert.NotEmpty(files);
            Assert.Equal(1, files.Count);
            Assert.Equal("Item 1", files[0].Name);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task ListFilesOverMultipleRequests()
        {
            var settings = new Settings { DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken") };

            var httpClient = Substitute.For<IHttpClientHelper>();

            var dropboxResponse = new DropboxApiResponseListFiles
                                  {
                                      has_more = true,
                                      cursor = "mycursor",
                                      entries = new[]
                                                {
                                                    new Entry { name = "Item 1" },
                                                    new Entry { name = "Item 2" },
                                                    new Entry { name = "Item 3" },
                                                }
                                  };

            var dropboxResponse2 = new DropboxApiResponseListFiles
                                  {
                                      has_more = false,
                                      cursor = "",
                                      entries = new[]
                                                {
                                                    new Entry { name = "Item 4" },
                                                    new Entry { name = "Item 5" },
                                                }
            };

            httpClient.PostAsync(new Uri("https://api.dropboxapi.com/2/files/list_folder"), @"{""path"":""""}", Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(JsonConvert.SerializeObject(dropboxResponse)));

            httpClient.PostAsync(new Uri("https://api.dropboxapi.com/2/files/list_folder/continue"), @"{""cursor"": ""mycursor""}", Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(JsonConvert.SerializeObject(dropboxResponse2)));

            var sut = new DropboxHelper(settings, httpClient);

            var files = await sut.GetFilesAsync("", CancellationToken.None);

            Assert.NotEmpty(files);
            Assert.Equal(5, files.Count);
            Assert.Equal("Item 1", files[0].Name);
            Assert.Equal("Item 2", files[1].Name);
            Assert.Equal("Item 3", files[2].Name);
            Assert.Equal("Item 4", files[3].Name);
            Assert.Equal("Item 5", files[4].Name);
        }
    }
}
