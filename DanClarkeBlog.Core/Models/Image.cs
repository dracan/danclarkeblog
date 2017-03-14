using System;

namespace DanClarkeBlog.Core.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ImageToken { get; set; }

        public Image()
        {
            Id = Guid.NewGuid();
        }
    }
}