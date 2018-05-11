using System;

namespace DanClarkeBlog.Core.Models
{
    /// <summary>
    /// This entity is currently required as EFCore doesn't yet support many-to-many relationships
    /// where the interim table is automatically generated.
    /// </summary>
    public class BlogPostTag
    {
        public Guid BlogPostId { get; set; }
        public string TagName { get; set; }

        public virtual BlogPost BlogPost { get; set; }
        public virtual Tag Tag { get; set; }

        public BlogPostTag()
        {
        }

        public BlogPostTag(BlogPost blogPost, Tag tag)
        {
            BlogPostId = blogPost.Id;
            TagName = tag.Name;

            BlogPost = blogPost;
            Tag = tag;
        }
    }
}