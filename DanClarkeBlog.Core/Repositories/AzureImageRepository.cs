using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Serilog;

namespace DanClarkeBlog.Core.Repositories
{
    public class AzureImageRepository : IImageRepository
    {
        private readonly Settings _settings;

	    public AzureImageRepository(Settings settings)
	    {
		    _settings = settings;
	    }

        public async Task AddAsync(string destPath, string fileName, byte[] data, CancellationToken cancellationToken)
        {
            // We only want the last part of the path, as we might have temprarily put the blog post
            // in a sub folder (eg. a shared "WIP" Dropbox folder).
            var leafPostFolderName = new DirectoryInfo(destPath).Name;

            var fileReference = $"{leafPostFolderName.TrimStart('/')}/{fileName}";

            Log.Debug($"AzureImageRepository.AddAsync called for image {fileReference}");

            var storageAccount = CreateStorageAccountFromConnectionString(_settings.AzureStorageConnectionString);

            var blogClient = storageAccount.CreateCloudBlobClient();

            var container = blogClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null, cancellationToken);

            var blobReference = container.GetBlockBlobReference(fileReference.ToLower());

            var dataHash = GenerateHash(data);

            if (await blobReference.ExistsAsync(cancellationToken))
            {
                if (blobReference.Metadata.ContainsKey("CRC") && blobReference.Metadata["CRC"] == dataHash)
                {
                    Log.Debug("File already exists, and hashes match, so not attempting upload");
                    return;
                }

                Log.Debug("File already exists, but hashes differ, so attempting upload");
            }
            else
            {
                Log.Debug($"File does not exist, so attempting upload (data length = {data.Length}) ...");
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