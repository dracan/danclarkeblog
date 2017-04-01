using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NLog;

namespace DanClarkeBlog.Core.Repositories
{
    public class AzureImageRepository : IImageRepository
    {
        private readonly Settings _settings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AzureImageRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task AddAsync(string fileReference, byte[] data)
        {
            var storageAccount = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);

            var blogClient = storageAccount.CreateCloudBlobClient();

            var container = blogClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            var blobReference = container.GetBlockBlobReference(fileReference);

            if(!await blobReference.ExistsAsync())
            {
                await blobReference.UploadFromByteArrayAsync(data, 0, data.Length);
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