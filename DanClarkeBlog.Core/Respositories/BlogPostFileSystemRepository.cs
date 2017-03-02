using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Newtonsoft.Json;
using NLog;

namespace DanClarkeBlog.Core.Respositories
{
    public class BlogPostFileSystemRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly Settings _settings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogPostFileSystemRepository(IBlogPostRenderer renderer, Settings settings)
        {
            _renderer = renderer;
            _settings = settings;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            _logger.Debug($"Processing files from filesystem (rootPath = {_settings.BlogFileSystemRootPath}) ...");

            var blogPosts = new List<BlogPost>();

            _logger.Debug("Reading blog.json ...");

            var content = File.ReadAllText(Path.Combine(_settings.BlogFileSystemRootPath, "Blog.json"));

            _logger.Trace($"Blog.json content was {content}");

            var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(content);

            _logger.Trace($"Enumerating through {blogPostList.Count} posts downloading the file contents ...");

            foreach (var blogPost in blogPostList)
            {
                var postFile = File.ReadAllText(Path.Combine(_settings.BlogFileSystemRootPath, blogPost.FilePath.TrimStart('/')));

                _logger.Trace($"Reading content for {blogPost.FilePath} ...");

                blogPosts.Add(new BlogPost
                {
                    Title = blogPost.Title,
                    PublishDate = DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                    HtmlText = _renderer.Render(postFile),
                    Route = blogPost.Route,
                    Tags = blogPost.Tags.Split('|').ToList()
                });
            }

            return blogPosts;
        }
    }
}