using System.Collections.Generic;

namespace DanClarkeBlog.Core.Models
{
    public class BlogPostListing
    {
        /// <summary>
        /// Posts in this page
        /// </summary>
        public List<BlogPost> Posts { get; set; }

        /// <summary>
        /// Total posts in the entire datastore
        /// </summary>
        public int TotalPosts;
    }
}