using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DanClarkeBlog.Worker.PostUpdated
{
    internal class PostUpdatedHandler : IRequestHandler<PostUpdatedRequest>
    {
        private readonly ILogger<PostUpdatedHandler> _logger;
        private readonly SyncHelper _syncHelper;
        private readonly IBlogPostSourceRepository _sourceRepository;
        private readonly IBlogPostTargetRepository _targetRepository;

        public PostUpdatedHandler(
            ILogger<PostUpdatedHandler> logger,
            SyncHelper syncHelper,
            IBlogPostSourceRepository sourceRepository,
            IBlogPostTargetRepository targetRepository)
        {
            _logger = logger;
            _syncHelper = syncHelper;
            _sourceRepository = sourceRepository;
            _targetRepository = targetRepository;
        }

        public async Task<Unit> Handle(PostUpdatedRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received blog post edited message, with IsIncremental = {IsIncremental}", request.Message.IsIncremental);

            (_targetRepository as BlogPostSqlServerRepository)?.CreateDatabase();

            await _syncHelper.SynchronizeBlogPostsAsync(
                (IBlogPostRepository)_sourceRepository,
                (IBlogPostRepository)_targetRepository,
                request.Message.IsIncremental,
                null,
                CancellationToken.None);

            _logger.LogInformation("Finished dropbox sync");

            return Unit.Value;
        }
    }
}