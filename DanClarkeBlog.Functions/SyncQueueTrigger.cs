using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DanClarkeBlog.Functions
{
    public class SyncQueueTrigger
    {
        private readonly SyncHelper _syncHelper;
        private readonly IBlogPostSourceRepository _sourceRepository;
        private readonly IBlogPostTargetRepository _targetRepository;

        public SyncQueueTrigger(SyncHelper syncHelper, IBlogPostSourceRepository sourceRepository, IBlogPostTargetRepository targetRepository)
        {
            _syncHelper = syncHelper;
            _sourceRepository = sourceRepository;
            _targetRepository = targetRepository;
        }

        [FunctionName("QueueTrigger")]
        public async Task Run([QueueTrigger("sync", Connection = "")] string message, ILogger log)
        {
            log.LogDebug("Found message on queue: {Message}", message);

            var msgObj = JsonConvert.DeserializeObject<SyncMessage>(message);

            (_targetRepository as BlogPostSqlServerRepository)?.CreateDatabase();

            await _syncHelper.SynchronizeBlogPostsAsync(
                (IBlogPostRepository)_sourceRepository,
                (IBlogPostRepository)_targetRepository,
                msgObj.IsIncremental,
                null,
                CancellationToken.None);

            Log.Debug("Finished dropbox sync");
        }
    }
}