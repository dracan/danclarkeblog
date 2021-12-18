using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DanClarkeBlog.Core.Repositories
{
    [UsedImplicitly]
    public class AzureImageRepository : IImageRepository
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public AzureImageRepository(IOptions<Settings> settings, ILogger<AzureImageRepository> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task AddAsync(string destPath, string fileName, byte[] data, CancellationToken cancellationToken)
        {
            // We only want the last part of the path, as we might have temprarily put the blog post
            // in a sub folder (eg. a shared "WIP" Dropbox folder).
            var leafPostFolderName = new DirectoryInfo(destPath).Name;

            var fileReference = $"{leafPostFolderName.TrimStart('/')}/{fileName}";

            _logger.LogInformation($"AzureImageRepository.AddAsync called for image {fileReference}");

            var storageAccount = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);

            var blogClient = storageAccount.CreateCloudBlobClient();

            var container = blogClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null, cancellationToken);

            var blobReference = container.GetBlockBlobReference(fileReference.ToLower());

            var dataHash = GenerateHash(data);

            if (await blobReference.ExistsAsync())
            {
                if (blobReference.Metadata.ContainsKey("CRC") && blobReference.Metadata["CRC"] == dataHash)
                {
                    _logger.LogInformation("File already exists, and hashes match, so not attempting upload");
                    return;
                }

                _logger.LogInformation("File already exists, but hashes differ, so attempting upload");
            }
            else
            {
                _logger.LogInformation($"File does not exist, so attempting upload (data length = {data.Length}) ...");
            }

            await blobReference.UploadFromByteArrayAsync(data, 0, data.Length);

            blobReference.Metadata["CRC"] = dataHash;
            await blobReference.SetMetadataAsync();
        }

        private static string GenerateHash(byte[] source)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha1.ComputeHash(source));
            }
        }

        /// <summary>
        /// Validates the connection string information in app.config and throws an exception if it looks like
        /// the user hasn't updated this to valid values.
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string</param>
        /// <returns>CloudStorageAccount object</returns>
        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;

            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
            catch (ArgumentException)
            {
                _logger.LogError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }
    }
}