using System;
using System.Collections.Generic;

namespace DanClarkeBlog.Core.Models
{
    public class BlogPost
    {
        public string Title { get; set; }
        public string Route { get; set; }
        public DateTime PublishDate { get; set; }
        public string HtmlText { get; set; }
        public string HtmlShortText { get; set; }
        public List<string> Tags { get; set; }
    }
}