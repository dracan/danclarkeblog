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

        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo, IBlogPostRepository destRepo, bool incremental, string overrideCursor, CancellationToken cancellationToken)
        {
            _logger.Trace($"SynchronizeBlogPostsAsync with incremental = {incremental}");

            var dropboxCursor = new CursorContainer();

            if (incremental)
            {
                // Try to get a persisted cursor from our SQL database, if that's null (so we haven't got one), then we'll do a full update
                dropboxCursor.Cursor = overrideCursor ?? await destRepo.GetDropboxCursorAsync(cancellationToken);

                _logger.Trace($"Cursor = {dropboxCursor.Cursor}");
            }

            var cursor = incremental && !string.IsNullOrWhiteSpace(dropboxCursor.Cursor) ? dropboxCursor : null;
            var sourcePosts = (await sourceRepo.GetAllAsync(cursor, cancellationToken)).ToList();

            if (incremental && string.IsNullOrWhiteSpace(dropboxCursor.Cursor))
            {
                _logger.Trace($"First incremental run, so explicitly requesting current cursor ...");
                dropboxCursor.Cursor = await _dropboxHelper.GetCurrentCursorAsync(cancellationToken);
                _logger.Trace($"Returned cursor {dropboxCursor.Cursor}");
            }

            _logger.Trace($"Processing {sourcePosts.Count} source posts ...");

            foreach(var sourcePost in sourcePosts)
            {
                await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);
            }

            if (!incremental) // Do not delete posts when in incremental mode (todo) Is this comment correct? Surely as we're reading the json file even on incremental sync, we can still delete on incremental?
            {
                //(todo) GetAllAsync filters by Published posts - we don't want this here. I think the GetAllAsync shouldn't unless explicitly requested
                var destPosts = (await destRepo.GetAllAsync(null, cancellationToken)).ToList();

                var postsToDelete = destPosts.Where(d => sourcePosts.All(s => s.Title != d.Title)).ToList();

                _logger.Trace($"Found {postsToDelete.Count} to delete out of {destPosts.Count} posts");

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