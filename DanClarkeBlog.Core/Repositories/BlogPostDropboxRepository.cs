using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Repositories
{
    [UsedImplicitly]
    public class BlogPostDropboxRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly BlogPostSummaryHelper _blogPostSummaryHelper;
        private readonly IDropboxHelper _dropboxHelper;
        private readonly ILogger _logger;

        private static readonly Func<DropboxFileModel, bool> ImageFileFilter = x => new[] { ".jpg", ".png", ".gif" }.Any(x.Name.Contains);

        public BlogPostDropboxRepository(IBlogPostRenderer renderer,
                                         BlogPostSummaryHelper blogPostSummaryHelper,
                                         IDropboxHelper dropboxHelper,
                                         ILogger<BlogPostDropboxRepository> logger)
        {
            _renderer = renderer;
            _blogPostSummaryHelper = blogPostSummaryHelper;
            _dropboxHelper = dropboxHelper;
            _logger = logger;
        }

        public Task<BlogPostListing> GetPublishedAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CursorContainer cursor, CancellationToken cancellationToken)
        {
            List<DropboxFileModel> dropboxFiles = null;

            if (cursor == null)
            {
                _logger.LogDebug("Processing files from Dropbox ...");
            }
            else
            {
                _logger.LogDebug("Processing updated files from Dropbox ...");

                dropboxFiles = await _dropboxHelper.GetFilesAsync("", cursor, cancellationToken);

                _logger.LogDebug("Files dropbox thinks has been updated:");

                foreach (var updatedFile in dropboxFiles)
                    _logger.LogDebug($"  Name: \"{updatedFile.Name}\", PathLower: \"{updatedFile.PathLower}\"");
            }

            var blogPosts = new List<BlogPost>();

            _logger.LogDebug("Reading blog.json ...");

            var blogMetaDataFile = await _dropboxHelper.GetFileContentAsync("/Blog.json", cancellationToken);

            var blogJson = Encoding.UTF8.GetString(blogMetaDataFile);

            _logger.LogTrace($"Blog.json content was {blogJson}");

            var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(blogJson);

            var blogPostsToUpdate = cursor == null
                ? blogPostList
                : blogPostList.Where(x => dropboxFiles.Any(y => y.PathLower == $"{x.Folder}/post.md".ToLower())).ToList();

            _logger.LogDebug($"Enumerating through {blogPostsToUpdate.Count} posts downloading the file contents ...");

            foreach (var blogPost in blogPostsToUpdate)
            {
                var imagePath = $"{blogPost.Folder}/images/";

                //(todo) Refactor so that in the case of an incremental sync, I'm only getting images that have changed since the cursor.

                var images = (await _dropboxHelper.GetFilesAsync(imagePath, cancellationToken)).Where(ImageFileFilter)
                    .Select(x => $"{imagePath}{x.Name}")
                    .Select(i => new BlogImageData
                    {
                        FileName = Path.GetFileName(i),
                        PostFolder = blogPost.Folder,
                        ImageDataTask = _dropboxHelper.GetFileContentAsync(i, cancellationToken),
                    }).ToList();

                _logger.LogDebug($"Reading content for {blogPost.Folder} ...");

                var postFile = await _dropboxHelper.GetFileContentAsync($"{blogPost.Folder}/post.md", cancellationToken);

                var postFileText = Encoding.UTF8.GetString(postFile);

                var post = new BlogPost
                           {
                               Id = blogPost.Id,
                               Title = blogPost.Title,
                               PublishDate = string.IsNullOrWhiteSpace(blogPost.PublishDate) ? null : (DateTime?)DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                               HtmlText = _renderer.Render(postFileText, blogPost.Folder),
                               HtmlShortText = _renderer.Render(_blogPostSummaryHelper.GetSummaryText(postFileText), blogPost.Folder),
                               Route = blogPost.Route,
                               Featured = blogPost.Featured,
                               Published = blogPost.Status.ToLower() == "published",
                               ImageData = images
                           };

                post.BlogPostTags = blogPost.Tags.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new BlogPostTag(post, new Tag(x))).ToList();

                blogPosts.Add(post);
            }

            return blogPosts;
        }

        public Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> GetDraftByIdAsync(Guid draftId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> GetPublishedByRouteAsync(string route, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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