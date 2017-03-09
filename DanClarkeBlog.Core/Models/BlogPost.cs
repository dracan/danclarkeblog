using System;
using System.Collections.Generic;

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
        public virtual ICollection<Tag> Tags { get; set; }

        public BlogPost()
        {
            Id = Guid.NewGuid();
        }
    }
}