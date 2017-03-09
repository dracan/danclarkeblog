using System;
using System.Collections.Generic;

namespace DanClarkeBlog.Core.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Tag(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}