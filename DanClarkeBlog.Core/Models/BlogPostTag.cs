using System;

namespace DanClarkeBlog.Core.Models
{
    /// <summary>
    /// This entity is currently required as EFCore doesn't yet support many-to-many relationships
    /// where the interim table is automaticaly generated.
    /// </summary>
    public class BlogPostTag
    {
        public Guid BlogPostId { get; set; }
        public Guid TagId { get; set; }

        public virtual BlogPost BlogPost { get; set; }
        public virtual Tag Tag { get; set; }

        public BlogPostTag(BlogPost blogPost, Tag tag)
        {
            BlogPostId = blogPost.Id;
            TagId = tag.Id;

            BlogPost = blogPost;
            Tag = tag;
        }
    }
}