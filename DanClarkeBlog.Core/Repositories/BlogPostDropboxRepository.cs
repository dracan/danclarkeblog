using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Newtonsoft.Json;

namespace DanClarkeBlog.Core.Repositories
{
    public class BlogPostDropboxRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly Settings _settings;
        private readonly BlogPostSummaryHelper _blogPostSummaryHelper;
        private readonly IImageRepository _imageRepository;
        private readonly IDropboxHelper _dropboxHelper;
        private readonly IImageResizer _imageResizer;

        private static readonly Func<DropboxFileModel, bool> ImageFileFilter = x => new[] { ".jpg", ".png", ".gif" }.Any(x.Name.Contains);

        public BlogPostDropboxRepository(IBlogPostRenderer renderer,
                                         Settings settings,
                                         BlogPostSummaryHelper blogPostSummaryHelper,
                                         IImageRepository imageRepository,
                                         IDropboxHelper dropboxHelper,
                                         IImageResizer imageResizer)
        {
            _renderer = renderer;
            _settings = settings;
            _blogPostSummaryHelper = blogPostSummaryHelper;
            _imageRepository = imageRepository;
            _dropboxHelper = dropboxHelper;
            _imageResizer = imageResizer;
        }

        public Task<BlogPostListing> GetAllAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<BlogPost>> GetUpdatesAsync(CursorContainer cursor, CancellationToken cancellationToken)
        {
            var updatedFiles = await _dropboxHelper.GetFilesAsync("", cursor, cancellationToken);

            //_logger.Debug("Processing updated files from Dropbox ...");

            var blogPosts = new List<BlogPost>();

            //_logger.Debug("Reading blog.json ...");

            var blogMetaDataFile = await _dropboxHelper.GetFileContentAsync("/Blog.json", cancellationToken);

            var blogJson = Encoding.UTF8.GetString(blogMetaDataFile);

            //_logger.Trace($"Blog.json content was {blogJson}");

            var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(blogJson);

            //_logger.Trace($"Enumerating through {blogPostList.Count} posts downloading the file contents ...");

            foreach (var blogPost in blogPostList.Where(x => updatedFiles.Any(y => y.PathLower == x.FilePath.ToLower())))
            {
                var imageFiles = (await _dropboxHelper.GetFilesAsync(blogPost.ImagePath, cancellationToken)).Where(ImageFileFilter);

                foreach (var image in imageFiles.Select(x => Path.Combine(blogPost.ImagePath, x.Name)))
                {
                    var imageFileContent = await _dropboxHelper.GetFileContentAsync(image, cancellationToken);

                    var resizedImageFileContent = _imageResizer.Resize(imageFileContent, 800);

                    await _imageRepository.AddAsync(Regex.Replace(image, @"/images/", ""), resizedImageFileContent);
                }

                //_logger.Trace($"Reading content for {blogPost.FilePath} ...");

                var postFile = await _dropboxHelper.GetFileContentAsync(blogPost.FilePath, cancellationToken);

                var postFileText = Encoding.UTF8.GetString(postFile);

                var post = new BlogPost
                           {
                               Title = blogPost.Title,
                               PublishDate = DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                               HtmlText = _renderer.Render(postFileText),
                               HtmlShortText = _renderer.Render(_blogPostSummaryHelper.GetSummaryText(postFileText)),
                               Route = blogPost.Route,
                               Featured = blogPost.Featured,
                               Published = blogPost.Status.ToLower() == "published"
                           };

                post.BlogPostTags = blogPost.Tags.Split('|').Select(x => new BlogPostTag(post, new Tag(x))).ToList();

                blogPosts.Add(post);
            }

            return blogPosts;
        }

        public Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken)
        {
            //var files = await _dropboxHelper.GetFilesAsync(cancellationToken);

            //_logger.Debug("Processing files from Dropbox ...");

            var blogPosts = new List<BlogPost>();

            //_logger.Debug("Reading blog.json ...");

            var blogMetaDataFile = await _dropboxHelper.GetFileContentAsync("/Blog.json", cancellationToken);

            var blogJson = Encoding.UTF8.GetString(blogMetaDataFile);

            //_logger.Trace($"Blog.json content was {blogJson}");

            var blogPostList = JsonConvert.DeserializeObject<List<BlogJsonItem>>(blogJson);

            //_logger.Trace($"Enumerating through {blogPostList.Count} posts downloading the file contents ...");

            foreach (var blogPost in blogPostList)
            {
                var imageFiles = (await _dropboxHelper.GetFilesAsync(blogPost.ImagePath, cancellationToken)).Where(ImageFileFilter);

                foreach (var image in imageFiles.Select(x => Path.Combine(blogPost.ImagePath, x.Name)))
                {
                    var imageFileContent = await _dropboxHelper.GetFileContentAsync(image, cancellationToken);

                    var resizedImageFileContent = _imageResizer.Resize(imageFileContent, 800);

                    await _imageRepository.AddAsync(Regex.Replace(image, @"/images/", ""), resizedImageFileContent);
                }

                //_logger.Trace($"Reading content for {blogPost.FilePath} ...");

                var postFile = await _dropboxHelper.GetFileContentAsync(blogPost.FilePath, cancellationToken);

                var postFileText = Encoding.UTF8.GetString(postFile);

                var post = new BlogPost
                           {
                               Title = blogPost.Title,
                               PublishDate = DateTime.ParseExact(blogPost.PublishDate, "yyyy-MM-dd", new CultureInfo("en-GB")),
                               HtmlText = _renderer.Render(postFileText),
                               HtmlShortText = _renderer.Render(_blogPostSummaryHelper.GetSummaryText(postFileText)),
                               Route = blogPost.Route,
                               Featured = blogPost.Featured,
                               Published = blogPost.Status.ToLower() == "published"
                           };

                post.BlogPostTags = blogPost.Tags.Split('|').Select(x => new BlogPostTag(post, new Tag(x))).ToList();

                blogPosts.Add(post);
            }

            return blogPosts;
        }

        public Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

        public Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken)
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