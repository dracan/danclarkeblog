using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DanClarkeBlog.Core.Repositories
{
    public class AzureImageRepository : IImageRepository
    {
        private readonly Settings _settings;
	    private readonly ILogger _logger;

	    public AzureImageRepository(Settings settings, ILogger logger)
	    {
		    _settings = settings;
		    _logger = logger;
	    }

        public async Task AddAsync(string fileReference, byte[] data, CancellationToken cancellationToken)
        {
            _logger.Debug($"AzureImageRepository.AddAsync called for image {fileReference}");

            var storageAccount = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);

            var blogClient = storageAccount.CreateCloudBlobClient();

            var container = blogClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null, cancellationToken);

            var blobReference = container.GetBlockBlobReference(fileReference);

            var dataHash = GenerateHash(data);

            if (await blobReference.ExistsAsync(cancellationToken))
            {
                if (blobReference.Metadata.ContainsKey("CRC") && blobReference.Metadata["CRC"] == dataHash)
                {
                    _logger.Debug("File already exists, and hashes match, so not attempting upload");
                    return;
                }

                _logger.Debug("File already exists, but hashes differ, so attempting upload");
            }
            else
            {
                _logger.Debug($"File does not exist, so attempting upload (data length = {data.Length}) ...");
            }

            await blobReference.UploadFromByteArrayAsync(data, 0, data.Length, cancellationToken);

            blobReference.Metadata["CRC"] = dataHash;
            await blobReference.SetMetadataAsync(cancellationToken);
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
                _logger.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
            catch (ArgumentException)
            {
                _logger.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }
    }
}