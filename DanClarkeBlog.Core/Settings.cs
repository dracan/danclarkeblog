﻿namespace DanClarkeBlog.Core
{
    public class Settings
    {
        public string DropboxAccessToken { get; set; }
        public string BlogSqlConnectionString { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string AzureServiceBusConnectionString { get; set; }
        public string DisqusDomainName { get; set; }
        public int MaxResizedImageSize { get; set; }
        public string KeepAlivePingUri { get; set; }
        public string SlackNotificationUri { get; set; }
        public string SiteHomeUri { get; set; }
        public string ProfilePicUri { get; set; }
        public int PostPreviewLength { get; set; } = 200;
        public string BaseImageUri { get; set; }
        public string GoogleTagId { get; set; }

        public string VersionNumber { get; set; }

        /// <summary>
        /// For verification of Dropbox webhook HMAC
        /// </summary>
        public string DropboxAppSecret { get; set; }

        /// <summary>
        /// For use locally to access Dropbox folder directly without going through the API
        /// Ie. The BlogPostFileSystemRepository implementation
        /// </summary>
        public string BlogFileSystemRootPath { get; set; }

        // Azure Search settings
        public string AzureSearchInstanceName { get; set; }
        public string AzureSearchKey { get; set; }
        public string AzureSearchIndexName { get; set; }
    }
}
