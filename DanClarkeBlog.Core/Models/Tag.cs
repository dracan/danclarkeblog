using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DanClarkeBlog.Core.Models
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }

        public virtual ICollection<BlogPostTag> BlogPostTags { get; set; }

        public Tag()
        {
        }

        public Tag(string name)
        {
            Name = name;
        }
    }
}