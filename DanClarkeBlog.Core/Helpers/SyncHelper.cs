using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Repositories;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class SyncHelper
    {
        private readonly IDropboxHelper _dropboxHelper;
        private readonly IImageRepository _imageRepository;
        private readonly IImageResizer _imageResizer;
        private readonly ILockRepository _lockRepository;
        private readonly ILogger _logger;
        private readonly Settings _settings;

        public SyncHelper(IDropboxHelper dropboxHelper,
                          IImageRepository imageRepository,
                          IImageResizer imageResizer,
                          ILockRepository lockRepository,
                          ILogger<SyncHelper> logger,
                          Settings settings)
        {
            _dropboxHelper = dropboxHelper;
            _imageRepository = imageRepository;
            _imageResizer = imageResizer;
            _lockRepository = lockRepository;
            _logger = logger;
            _settings = settings;
        }

        public async Task SynchronizeBlogPostsAsync(IBlogPostRepository sourceRepo,
                                                    IBlogPostRepository destRepo,
                                                    bool incremental,
                                                    string overrideCursor,
                                                    CancellationToken cancellationToken)
        {
            try
            {
                await _lockRepository.AcquireLockAsync("synchelperlock", 10, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1), cancellationToken);

                _logger.LogInformation($"SynchronizeBlogPostsAsync with incremental = {incremental}");

                var dropboxCursor = new CursorContainer();

                if (incremental)
                {
                    // Try to get a persisted cursor from our SQL database, if that's null (so we haven't got one), then we'll do a full update
                    dropboxCursor.Cursor = overrideCursor ?? await destRepo.GetDropboxCursorAsync(cancellationToken);

                    _logger.LogDebug($"Cursor = {dropboxCursor.Cursor}");
                }

                var cursor = incremental && !string.IsNullOrWhiteSpace(dropboxCursor.Cursor) ? dropboxCursor : null;
                var sourcePosts = (await sourceRepo.GetAllAsync(cursor, cancellationToken)).ToList();

                if (string.IsNullOrWhiteSpace(dropboxCursor.Cursor))
                {
                    _logger.LogInformation("No current dropbox cursor, so explicitly requesting current cursor ...");
                    dropboxCursor.Cursor = await _dropboxHelper.GetCurrentCursorAsync(cancellationToken);
                    _logger.LogInformation($"Returned cursor {dropboxCursor.Cursor}");
                }

                _logger.LogInformation($"Processing {sourcePosts.Count} source posts ...");

                foreach (var sourcePost in sourcePosts)
                    await destRepo.AddOrUpdateAsync(sourcePost, cancellationToken);

                _logger.LogInformation("Processing images ...");

                var imageTasks = new List<Task>();

                foreach (var sourcePost in sourcePosts)
                {
                    foreach (var imageData in sourcePost.ImageData)
                    {
                        var imageContent = await imageData.ImageDataTask;

                        var resizedImageFileContent = imageData.FileName.ToLower().Contains("noresize")
                            ? imageContent
                            : _imageResizer.Resize(imageContent, _settings.MaxResizedImageSize);

                        imageTasks.Add(_imageRepository.AddAsync(imageData.PostFolder, imageData.FileName, resizedImageFileContent, cancellationToken));
                    }
                }

                await Task.WhenAll(imageTasks);

                if (!incremental) // Do not delete posts when in incremental mode (todo) Is this comment correct? Surely as we're reading the json file even on incremental sync, we can still delete on incremental?
                {
                    var destPosts = (await destRepo.GetAllAsync(null, cancellationToken)).ToList();

                    var postsToDelete = destPosts.Where(d => sourcePosts.All(s => s.Id != d.Id)).ToList();

                    _logger.LogInformation($"Found {postsToDelete.Count} to delete out of {destPosts.Count} posts");

                    await destRepo.DeleteAsync(postsToDelete, cancellationToken);
                }

                await destRepo.RemoveUnusedTagsAsync(cancellationToken);

                _logger.LogInformation($"Saving new Dropbox cursor: {dropboxCursor.Cursor}");

                await destRepo.SetDropboxCursorAsync(dropboxCursor.Cursor, cancellationToken);
            }
            finally
            {
                await _lockRepository.ReleaseLockAsync(cancellationToken);
            }
        }
    }
}