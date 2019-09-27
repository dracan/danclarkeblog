﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Serilog;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class AzureStorageQueue : IMessageQueue
    {
        private readonly Dictionary<string, CloudQueue> _initialisedQueues = new Dictionary<string, CloudQueue>();
        private CloudQueueClient _client;
        private readonly string _connectionString;

        public AzureStorageQueue(Settings settings)
        {
            _connectionString = settings.AzureStorageConnectionString;
        }

        private async Task<CloudQueue> GetQueueReferenceAsync(string queueName)
        {
            if (_client == null)
                _client = CloudStorageAccount.Parse(_connectionString)
                    .CreateCloudQueueClient();

            if (_initialisedQueues.TryGetValue(queueName, out var val))
                return val;

            var queueReference = _client.GetQueueReference(queueName);

            await queueReference.CreateIfNotExistsAsync();

            _initialisedQueues.Add(queueName, queueReference);

            return queueReference;
        }

        public async Task SendAsync(string queueName, string message, CancellationToken cancellationToken)
        {
            var queue = await GetQueueReferenceAsync(queueName);

            await queue.AddMessageAsync(new CloudQueueMessage(message), cancellationToken);

            Log.Debug("Sent message: {Message}", message);
        }

        public async Task SubscribeAsync(string queueName, Func<string, Task> callbackAsync, CancellationToken cancellationToken)
        {
            var queue = await GetQueueReferenceAsync(queueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                var msg = await queue.GetMessageAsync(cancellationToken);

                if (msg != null)
                {
                    await callbackAsync(msg.AsString);

                    Log.Information($"Message received for queue {queueName}");

                    await queue.DeleteMessageAsync(msg, cancellationToken);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        public void Dispose()
        {
        }
    }
}