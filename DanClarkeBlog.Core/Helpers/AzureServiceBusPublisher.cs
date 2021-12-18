using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class AzureServiceBusPublisher : IMessageQueue
    {
        private readonly IOptions<Settings> _settings;

        public AzureServiceBusPublisher(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        public async Task SendAsync(string topicName, string message, CancellationToken cancellationToken)
        {
            await using var client = new ServiceBusClient(_settings.Value.AzureServiceBusConnectionString);
            var sender = client.CreateSender(topicName);
            await sender.SendMessageAsync(new ServiceBusMessage(message), cancellationToken);
        }
    }
}