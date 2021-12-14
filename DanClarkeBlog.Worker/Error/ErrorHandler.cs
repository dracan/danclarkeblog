using MediatR;
using Microsoft.Extensions.Logging;

namespace DanClarkeBlog.Worker.Error;

public class ErrorHandler : IRequestHandler<ErrorRequest>
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(ErrorRequest request, CancellationToken cancellationToken)
    {
        _logger.LogError(request.Args.Exception, "Error receiving message from queue");

        return Task.FromResult(Unit.Value);
    }
}