namespace DanClarkeBlog.Core.Models
{
    public class BlogJsonItem
    {
        public string Title { get; set; }
        public string Folder { get; set; }
        public string Route { get; set; }
        public string Status { get; set; }
        public string PublishDate { get; set; }
        public string Tags { get; set; }
        public bool Featured { get; set; }
    }
}