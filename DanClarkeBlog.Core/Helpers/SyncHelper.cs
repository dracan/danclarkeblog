using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Repositories;

namespace DanClarkeBlog.Core.Helpers
{
    public class SyncHelper
    {
        private readonly ILogger _logger;
        private readonly IDropboxHelper _dropboxHelper;

        public SyncHelper(ILogger logger, IDropboxHelper dropboxHelper)
        {
            _logger = logger;
            _dropboxHelper = dropboxHelper;
        }

        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, bool incremental, CancellationToken cancellationToken)
        {
            _logger.Trace($"SynchronizeBlogPostsAsync with incremental = {incremental}");

            var dropboxCursor = new CursorContainer();

            if (incremental)
            {
                // Try to get a persisted cursor from our SQL database, if that's null (so we haven't got one), then we'll do a full update
                dropboxCursor.Cursor = await destRepo.GetDropboxCursorAsync(cancellationToken);

                _logger.Trace($"Cursor = {dropboxCursor.Cursor}");
            }

            var sourcePosts = incremental && !string.IsNullOrWhiteSpace(dropboxCursor.Cursor)
                ? (await sourceRepo.GetUpdatesAsync(dropboxCursor, cancellationToken)).ToList()
                : (await sourceRepo.GetAllAsync(cancellationToken)).ToList();

            if (incremental && string.IsNullOrWhiteSpace(dropboxCursor.Cursor))
            {
                _logger.Trace($"First incremental run, so explicitly requesting current cursor ...");
                dropboxCursor.Cursor = await _dropboxHelper.GetCurrentCursorAsync(cancellationToken);
                _logger.Trace($"Returned cursor {dropboxCursor.Cursor}");
            }

            var destPosts = (await destRepo.GetAllAsync(cancellationToken)).ToList();

            _logger.Trace($"Processing {sourcePosts.Count} source posts, and {destPosts.Count()} dest posts");

            foreach(var sourcePost in sourcePosts)
            {
                await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);
            }

            if (!incremental) // Do not delete posts when in incremental mode
            {
                var postsToDelete = destPosts.Where(d => sourcePosts.All(s => s.Title != d.Title)).ToList();

                _logger.Trace($"Found {postsToDelete.Count} to delete");

                await destRepo.DeleteAsync(postsToDelete, cancellationToken);
            }

            await destRepo.RemoveUnusedTagsAsync(cancellationToken);

            if (incremental)
	        {
                _logger.Trace($"Saving new Dropbox cursor: {dropboxCursor.Cursor}");

		        await destRepo.SetDropboxCursorAsync(dropboxCursor.Cursor, cancellationToken);
	        }
        }
    }
}