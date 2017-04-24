namespace DanClarkeBlog.Core
{
    public class Settings
    {
        public string DropboxAccessToken { get; set; }
        public string BlogSqlConnectionString { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string DisqusDomainName { get; set; }
        public int MaxResizedImageSize { get; set; }
        public string KeepAlivePingUri { get; set; }
        public string SlackNotificationUri { get; set; }
        public string SiteHomeUri { get; set; }

        /// <summary>
        /// For verification of Dropbox webhook HMAC
        /// </summary>
        public string DropboxAppSecret { get; set; }

        /// <summary>
        /// For use locally to access Dropbox folder directly without going through the API
        /// Ie. The BlogPostFileSystemRepository implementation
        /// </summary>
        public string BlogFileSystemRootPath { get; set; }
    }
}
