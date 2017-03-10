using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Dropbox.Api;
using Newtonsoft.Json;
using NLog;

namespace DanClarkeBlog.Core.Respositories
{
    public class BlogPostDropboxRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly Settings _settings;
        private readonly BlogPostSummaryHelper _blogPostSummaryHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogPostDropboxRepository(IBlogPostRenderer renderer,
                                         Settings settings,
                                         BlogPostSummaryHelper blogPostSummaryHelper)
        {
            _renderer = renderer;
            _settings = settings;
            _blogPostSummaryHelper = blogPostSummaryHelper;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("Processing files from Dropbox ...");

            var blogPosts = new List<BlogPost>();

            using (var dropboxClient = new DropboxClient(_settings.DropboxAccessToken))
            {
                _logger.Debug("Reading blog.json ...");

                using (var blogJson = await dropboxClient.Files.DownloadAsync("/Blog.json"))
                {
                    var content = await blogJson.GetContentAsStringAsync();

                    _logger.Trace($"Blog.json content was {content}");

                    var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(content);

                    _logger.Trace($"Enumerating through {blogPostList.Count} posts downloading the file contents ...");

                    foreach (var blogPost in blogPostList)
                    {
                        using (var postFile = await dropboxClient.Files.DownloadAsync(blogPost.FilePath))
                        {
                            _logger.Trace($"Reading content for {blogPost.FilePath} ...");

                            content = await postFile.GetContentAsStringAsync();

                            blogPosts.Add(new BlogPost
                            {
                                Title = blogPost.Title,
                                PublishDate = DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                                HtmlText = _renderer.Render(content),
                                HtmlShortText = _renderer.Render(_blogPostSummaryHelper.GetSummaryText(content)),
                                Route = blogPost.Route,
                                Tags = blogPost.Tags.Split('|').Select(x => new Tag(x)).ToList()
                            });
                        }
                    }
                }

                return blogPosts;
            }
        }

        public Task AddAsync(BlogPost post, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}