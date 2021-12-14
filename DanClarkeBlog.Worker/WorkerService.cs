using Azure.Messaging.ServiceBus;
using DanClarkeBlog.Core;
using DanClarkeBlog.Worker.Error;
using DanClarkeBlog.Worker.Models;
using DanClarkeBlog.Worker.PostUpdated;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DanClarkeBlog.Worker;

public class WorkerService : IHostedService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IMediator _mediator;
    private readonly ServiceBusClient _client;
    private List<ServiceBusProcessor>? _processors;

    public WorkerService(ILogger<WorkerService> logger, IMediator mediator, IOptions<Settings> settings)
    {
        _logger = logger;
        _mediator = mediator;
        _client = new ServiceBusClient(settings.Value.AzureServiceBusConnectionString);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _processors = new List<ServiceBusProcessor>
        {
            await RegisterListenerAsync(
                "blog-postupdated",
                typeof(SyncMessage),
                msg => new PostUpdatedRequest((SyncMessage)msg),
                cancellationToken),
        };
    }

    private async Task<ServiceBusProcessor> RegisterListenerAsync(string topic, Type dtoType, Func<object, IRequest> createRequest, CancellationToken cancellationToken)
    {
        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        };

        var processor = _client.CreateProcessor(topic, "worker", options);

        processor.ProcessMessageAsync += async args =>
        {
            _logger.LogInformation("Processing message from topic {Topic}", topic);

            try
            {
                var msgObj = JsonConvert.DeserializeObject(args.Message.Body.ToString(), dtoType);
                await _mediator.Send(createRequest(msgObj), cancellationToken);
                await args.CompleteMessageAsync(args.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception when trying to process message from topic {Topic}", topic);
                await args.DeadLetterMessageAsync(args.Message, null, cancellationToken);
            }
        };

        processor.ProcessErrorAsync +=
            async args => await _mediator.Send(new ErrorRequest(args), cancellationToken);

        await processor.StartProcessingAsync(cancellationToken);

        return processor;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if(_processors != null)
            foreach (var processor in _processors)
                await processor.DisposeAsync();

        await _client.DisposeAsync();
    }
}