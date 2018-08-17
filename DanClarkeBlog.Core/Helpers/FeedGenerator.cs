using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DanClarkeBlog.Core.Repositories;
using JetBrains.Annotations;
using Terradue.ServiceModel.Syndication;

namespace DanClarkeBlog.Core.Helpers
{
    //(todo) A lot of hardcoded strings specific to my blog here need making into settings

    [UsedImplicitly]
    public class FeedGenerator : IFeedGenerator
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly Settings _settings;

        public FeedGenerator(IBlogPostRepository blogPostRepository, Settings settings)
        {
            _blogPostRepository = blogPostRepository;
            _settings = settings;
        }

        public async Task<string> GenerateRssAsync(CancellationToken cancelationToken)
        {
            var feed = await GenerateInternalAsync(cancelationToken);

            using (var ms = new MemoryStream())
            {
                using (var rssWriter = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    var rssFormatter = new Rss20FeedFormatter(feed);
                    rssFormatter.WriteTo(rssWriter);
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> GenerateAtomAsync(CancellationToken cancelationToken)
        {
            var feed = await GenerateInternalAsync(cancelationToken);

            using (var ms = new MemoryStream())
            {
                using (var atomWriter = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    var atomFormatter = new Atom10FeedFormatter(feed);
                    atomFormatter.WriteTo(atomWriter);
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task<SyndicationFeed> GenerateInternalAsync(CancellationToken cancelationToken)
        {
            var blogDescription = "Dan Clarke's Blog";

            var feed = new SyndicationFeed
            {
                Id = _settings.SiteHomeUri,
                Title = new TextSyndicationContent(blogDescription),
                Description = new TextSyndicationContent(blogDescription),
                LastUpdatedTime = DateTimeOffset.Now,
                Copyright = new TextSyndicationContent($"Copyright {DateTime.Now.Year}"),
                Generator = "Dan Clarke's Blog Platform",
                Language = "en-gb",
                Categories = { new SyndicationCategory("Programming") },
            };

            feed.Authors.Add(new SyndicationPerson("", "Dan Clarke", _settings.SiteHomeUri));
            feed.Categories.Add(new SyndicationCategory("FeedCategory", "CategoryScheme", "CategoryLabel"));

            feed.Links.Add(new SyndicationLink(new Uri(new Uri(_settings.SiteHomeUri), "/rss"))
            {
                RelationshipType = "self",
                MediaType = "text/html",
                Title = blogDescription
            });

            feed.Links.Add(new SyndicationLink(new Uri(_settings.SiteHomeUri))
            {
                MediaType = "text/html",
                Title = blogDescription
            });

            var posts = await _blogPostRepository.GetWithConditionAsync(x => x.Published, cancelationToken);

            var items = new List<SyndicationItem>();

            foreach (var post in posts)
            {
                var postUri = new Uri(new Uri(_settings.SiteHomeUri), post.Route);

                var item = new SyndicationItem
                {
                    Id = postUri.ToString(),
                    Title = new TextSyndicationContent(post.Title),
                    Content = new TextSyndicationContent(post.HtmlText, TextSyndicationContentKind.Html)
                };

                if (post.PublishDate.HasValue)
                {
                    item.PublishDate = post.PublishDate.Value;
                }

                item.Links.Add(new SyndicationLink(postUri));

                items.Add(item);
            }

            feed.Items = items;

            return feed;
        }
    }
}