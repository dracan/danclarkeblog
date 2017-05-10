using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Polly;

namespace DanClarkeBlog.Core.Repositories
{
    public class AzureBlobLockRepository : ILockRepository
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private string _leaseId;
        private CloudBlobContainer _storageContainer;

        public AzureBlobLockRepository(ILogger logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task AcquireLockAsync(string key, int numRetries, TimeSpan lockTimeout, CancellationToken cancellationToken)
        {
           _logger.Debug($"Acquiring lock for key {key} ...");

            var storage = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);
            var storageClient = storage.CreateCloudBlobClient();
            _storageContainer = storageClient.GetContainerReference(key);
            await _storageContainer.CreateIfNotExistsAsync(cancellationToken);
            await _storageContainer.FetchAttributesAsync(cancellationToken);

            await Policy.Handle<StorageException>().RetryAsync(numRetries, (e, n) =>
               {
                   _logger.Debug($"Failed to acquire lock ({e.Message}). Retrying ... (retry count {n})");
               }).ExecuteAsync(async () =>
               {
                   _leaseId = await _storageContainer.AcquireLeaseAsync(lockTimeout);
               });
        }

        public async Task ReleaseLockAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_leaseId))
            {
               _logger.Debug("Releasing lock ...");
                await _storageContainer.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(_leaseId), cancellationToken);
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