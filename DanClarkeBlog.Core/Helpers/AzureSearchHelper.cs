using System;
using System.Linq;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Core.Helpers
{
    public class AzureSearchHelper : ISearchHelper
    {
        private readonly Settings _settings;

        public AzureSearchHelper(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<BlogPostListing> SearchAsync(string searchTerm, int offset, int count)
        {
            using (var serviceClient = new SearchServiceClient(_settings.AzureSearchInstanceName, new SearchCredentials(_settings.AzureSearchKey)))
            using (var indexClient = serviceClient.Indexes.GetClient(_settings.AzureSearchIndexName))
            {
                var results = (await indexClient.Documents.SearchAsync(searchTerm, new SearchParameters
                {
                    Select = new[] {"id", "Title", "Route", "PublishDate", "HtmlShortText", "Published"},
                    Skip = offset,
                    Top = count,
                    IncludeTotalResultCount = true,
                    OrderBy = new[] { "PublishDate desc" },
                    Filter = "Published"
                }));

                return new BlogPostListing
                {
                    Posts = results.Results.Select(x => new BlogPost
                    {
                        Id = Guid.Parse((string) x.Document["id"]),
                        Title = (string) x.Document["Title"],
                        Route = (string) x.Document["Route"],
                        HtmlShortText = (string) x.Document["HtmlShortText"],
                    }).ToList(),
                    TotalPosts = results.Count.HasValue ? (int)results.Count : 0
                };
            }
        }
    }
}