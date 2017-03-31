namespace DanClarkeBlog.Core.Dropbox
{
    public class DropboxApiResponseListFiles
    {
        public Entry[] entries { get; set; }
        public string cursor { get; set; }
        public bool has_more { get; set; }
    }
}