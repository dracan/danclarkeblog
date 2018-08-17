using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Polly;
using Serilog;

namespace DanClarkeBlog.Core.Repositories
{
    [UsedImplicitly]
    public class AzureBlobLockRepository : ILockRepository
    {
        private readonly Settings _settings;
        private string _leaseId;
        private CloudBlobContainer _storageContainer;

        public AzureBlobLockRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task AcquireLockAsync(string key, int numRetries, TimeSpan waitBetweenRetries, TimeSpan lockTimeout, CancellationToken cancellationToken)
        {
           Log.Debug($"Acquiring lock for key {key} ...");

            var storage = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);
            var storageClient = storage.CreateCloudBlobClient();
            _storageContainer = storageClient.GetContainerReference(key);
            await _storageContainer.CreateIfNotExistsAsync();
            await _storageContainer.FetchAttributesAsync();

            await Policy.Handle<StorageException>().WaitAndRetryAsync(numRetries, n =>
               {
                   Log.Debug($"Failed to acquire lock. Retrying ... (retry count {n})");
                   return waitBetweenRetries;
               }).ExecuteAsync(async () =>
               {
                   _leaseId = await _storageContainer.AcquireLeaseAsync(lockTimeout);
               });
        }

        public async Task ReleaseLockAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_leaseId))
            {
                Log.Debug("Releasing lock ...");
                await _storageContainer.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(_leaseId));
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
                Log.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
            catch (ArgumentException)
            {
                Log.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }

    }
}