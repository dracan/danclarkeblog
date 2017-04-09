using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace DanClarkeBlog.Core.Models
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Route { get; set; }
        public DateTime PublishDate { get; set; }
        public string HtmlText { get; set; }
        public string HtmlShortText { get; set; }
        public bool Featured { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<BlogPostTag> BlogPostTags { get; set; }

        public List<string> Tags => BlogPostTags.Where(x => x.Tag != null).Select(x => x.Tag.Name).ToList();

        public BlogPost()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateFrom(BlogPost post)
        {
            Title = post.Title;
            Route = post.Route;
            PublishDate = post.PublishDate;
            HtmlText = post.HtmlText;
            HtmlShortText = post.HtmlShortText;
            Featured = post.Featured;
            Published = post.Published;

            //(todo) Update tags here too
        }
    }
}