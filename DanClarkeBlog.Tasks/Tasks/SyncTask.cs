using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Tasks.Models;
using Newtonsoft.Json;
using Serilog;

namespace DanClarkeBlog.Tasks.Tasks
{
    public class SyncTask : ITask
    {
        private readonly INotificationTarget _notificationTarget;
        private readonly SyncHelper _syncHelper;
        private readonly IComponentContext _container;

        public SyncTask(INotificationTarget notificationTarget, SyncHelper syncHelper, IComponentContext container)
        {
            _notificationTarget = notificationTarget;
            _syncHelper = syncHelper;
            _container = container;
        }

        public async Task ExecuteAsync(string message)
        {
            var ct = CancellationToken.None;

            try
            {
                Log.Debug("Found message on queue: {Message}", message);

                var msgObj = JsonConvert.DeserializeObject<SyncMessage>(message);

                var sourceRepo = _container.ResolveNamed<IBlogPostRepository>("Dropbox");
                var destRepo = _container.ResolveNamed<IBlogPostRepository>("SqlServer");

                (destRepo as BlogPostSqlServerRepository)?.CreateDatabase();

                await _syncHelper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, msgObj.IsIncremental, null, CancellationToken.None);

                Log.Debug("Finished dropbox sync");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred in the ProcessDropboxChange Azure Function: {ex}");
                await _notificationTarget.SendMessageAsync($"An exception occurred in the ProcessDropboxChange Azure Function: {ex}", ct);
                throw;
            }
        }
    }
}