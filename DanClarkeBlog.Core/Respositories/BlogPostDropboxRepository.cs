using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IImageRepository _imageRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogPostDropboxRepository(IBlogPostRenderer renderer,
                                         Settings settings,
                                         BlogPostSummaryHelper blogPostSummaryHelper,
                                         IImageRepository imageRepository)
        {
            _renderer = renderer;
            _settings = settings;
            _blogPostSummaryHelper = blogPostSummaryHelper;
            _imageRepository = imageRepository;
        }

        public Task<BlogPostListing> GetAllAsync(int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
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
                        var imageFiles = await dropboxClient.Files.ListFolderAsync(blogPost.ImagePath);

                        while(imageFiles.HasMore)
                        {
                            //(todo) Is this the right way of doing it? Does returning append the previous iteration?
                            imageFiles = await dropboxClient.Files.ListFolderContinueAsync(imageFiles.Cursor);
                        }

                        foreach(var image in imageFiles.Entries.Where(x => x.IsFile))
                        {
                            using (var imageFile = await dropboxClient.Files.DownloadAsync(image.PathLower))
                            {
                                await _imageRepository.AddAsync(Regex.Replace(image.PathLower, @"^/images/", ""), await imageFile.GetContentAsByteArrayAsync());
                            }
                        }

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
                                Tags = blogPost.Tags.Split('|').Select(x => new Tag(x)).ToList(),
                                Featured = blogPost.Featured
                            });
                        }
                    }
                }

                return blogPosts;
            }
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
    }
}