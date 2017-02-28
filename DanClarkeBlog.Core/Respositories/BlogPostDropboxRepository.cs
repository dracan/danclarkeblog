using System.Collections.Generic;
using System.Linq;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;

namespace DanClarkeBlog.Core.Respositories
{
    public class BlogPostDropboxRepository : IBlogPostRepository
    {
        private readonly IBlogPostRenderer _renderer;
        private static readonly Dictionary<string, BlogPost> _cache = new Dictionary<string, BlogPost>();

        private Settings _settings;

        public BlogPostDropboxRepository(IBlogPostRenderer renderer, Settings settings)
        {
            _renderer = renderer;
            _settings = settings;
        }

        public IEnumerable<BlogPost> GetAll()
        {
            var blogPosts = new List<string>();

            //todo: get blog posts from Dropbox

            //[here] // Add dropbox implementation

            return blogPosts.Select(x => new BlogPost
                                         {
                                            Title = "todo",
                                            DateString = "todo",
                                            HtmlText = _renderer.Render(x),
                                         });
        }
    }
}