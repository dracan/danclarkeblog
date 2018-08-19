using System;
using JetBrains.Annotations;

namespace DanClarkeBlog.Core.Models
{
    [UsedImplicitly]
    public class BlogJsonItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Folder { get; set; }
        public string Route { get; set; }
        public string Status { get; set; }
        public string PublishDate { get; set; }
        public string Tags { get; set; }
        public bool Featured { get; set; }
    }
}