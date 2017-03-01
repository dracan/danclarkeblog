using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Dropbox.Api;
using Dropbox.Api.Files;
using NLog;

namespace DanClarkeBlog.Core.Respositories
{
    public class BlogPostDropboxRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private readonly Settings _settings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BlogPostDropboxRepository(IBlogPostRenderer renderer, Settings settings)
        {
            _renderer = renderer;
            _settings = settings;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            _logger.Debug("Getting all blog posts from dropbox ...");

            using (var dropboxClient = new DropboxClient(_settings.DropboxAccessToken))
            {
                //[here] // Looks like the dropbox library is failing with the System error :/
                var result = await dropboxClient.Files.ListFolderAsync("", true, true);

                _logger.Debug($"Returned {result.Entries.Count} entries from dropbox (hasmore = {result.HasMore})");

                while (result.HasMore)
                {
                    result = await dropboxClient.Files.ListFolderContinueAsync(new ListFolderContinueArg(result.Cursor));

                    _logger.Debug($"After continue, we now have {result.Entries.Count} entries from dropbox (hasmore = {result.HasMore})");
                }

                var entries = (from x in result.Entries
                               where x.IsFile
                               select x.AsFile).ToList();

                _logger.Debug($"After filtering results for files, we now have {entries.Count} files returned. Downloading content for each ...");

                var blogPosts = new List<BlogPost>();

                foreach (var entry in entries)
                {
                    _logger.Debug($"Downloading content for {entry.PathLower} ...");

                    using (var download = await dropboxClient.Files.DownloadAsync(entry.PathLower))
                    {
                        var content = await download.GetContentAsStringAsync();

                        blogPosts.Add(new BlogPost
                                      {
                                          Title = "todo",
                                          DateString = "todo",
                                          HtmlText = _renderer.Render(content),
                                      });
                    }
                }

                return blogPosts;
            }
        }
    }
}