namespace DanClarkeBlog.Core
{
    public class Settings
    {
        public string DropboxAccessToken { get; set; }

        /// <summary>
        /// For use locally to access Dropbox folder directly without going through the API
        /// Ie. The BlogPostFileSystemRepository implementation
        /// </summary>
        public string BlogFileSystemRootPath { get; set; }
    }
}
