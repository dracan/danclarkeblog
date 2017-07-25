using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Newtonsoft.Json;
using Serilog;

namespace DanClarkeBlog.Core.Repositories
{
    public class BlogPostFileSystemRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly Settings _settings;
        private readonly BlogPostSummaryHelper _blogPostSummaryHelper;

	    public BlogPostFileSystemRepository(IBlogPostRenderer renderer,
                                            Settings settings,
                                            BlogPostSummaryHelper blogPostSummaryHelper)
        {
            _renderer = renderer;
            _settings = settings;
            _blogPostSummaryHelper = blogPostSummaryHelper;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CursorContainer cursor, CancellationToken cancellationToken)
        {
            return (await GetPublishedAsync(null, null, null, cancellationToken)).Posts;
        }

        public Task<BlogPostListing> GetPublishedAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            Log.Debug($"Processing files from filesystem (rootPath = {_settings.BlogFileSystemRootPath}) ...");

            var blogPosts = new List<BlogPost>();

            Log.Debug("Reading blog.json ...");

            var content = File.ReadAllText(Path.Combine(_settings.BlogFileSystemRootPath, "Blog.json"));

            Log.Verbose($"Blog.json content was {content}");

            var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(content);

            Log.Verbose($"Enumerating through {blogPostList.Count} posts downloading the file contents ...");

            var posts = blogPostList.AsQueryable();

            if (offset.HasValue)
            {
                posts = posts.Skip(offset.Value);
            }

            if (maxResults.HasValue)
            {
                posts = posts.Take(maxResults.Value);
            }

            foreach (var blogPost in posts)
            {
                var postFile = File.ReadAllText(Path.Combine(_settings.BlogFileSystemRootPath, blogPost.Folder.TrimStart('/')));

                Log.Verbose($"Reading content for {blogPost.Folder} ...");

                var post = new BlogPost
                {
                    Title = blogPost.Title,
                    PublishDate = DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                    HtmlText = _renderer.Render(postFile, blogPost.Folder),
                    HtmlShortText = _renderer.Render(_blogPostSummaryHelper.GetSummaryText(postFile), blogPost.Folder),
                    Route = blogPost.Route,
                    Featured = blogPost.Featured,
                    Published = blogPost.Status.ToLower() == "published"
                };

                post.BlogPostTags = blogPost.Tags.Split('|').Select(x => new BlogPostTag(post, new Tag(x))).ToList();

                blogPosts.Add(post);
            }

            return Task.FromResult(new BlogPostListing
            {
                Posts = blogPosts,
                TotalPosts = blogPostList.Count
            });
        }

        public async Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken)
        {
            return (await GetAllAsync(null, cancellationToken)).Where(x => x.Featured).ToList();
        }

        public async Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken)
        {
            return (await GetAllAsync(null, cancellationToken)).Where(conditionFunc).ToList();
        }

        public Task AddAsync(BlogPost post, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken)
        {
            return (await GetAllAsync(null, cancellationToken)).Take(numRecent).ToList();
        }

        public Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task RemoveUnusedTagsAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetDropboxCursorAsync(string cursor, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetDropboxCursorAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}