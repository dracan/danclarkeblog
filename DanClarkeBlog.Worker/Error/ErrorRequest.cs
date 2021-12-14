using Azure.Messaging.ServiceBus;
using MediatR;

namespace DanClarkeBlog.Worker.Error;

public record ErrorRequest(ProcessErrorEventArgs Args) : IRequest<Unit>;