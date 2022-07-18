using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Data;
using DanClarkeBlog.Core.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanClarkeBlog.Core.Repositories
{
    [UsedImplicitly]
    public class BlogPostSqlServerRepository : IBlogPostRepository
    {
        private readonly Settings _setting;
        private readonly ILogger _logger;

        public BlogPostSqlServerRepository(IOptions<Settings> setting, ILogger<BlogPostSqlServerRepository> logger)
        {
            _setting = setting.Value;
            _logger = logger;
        }

        public void CreateDatabase()
        {
            _logger.LogInformation("Ensuring database exists...");

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
                ctx.Database.EnsureCreated();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CursorContainer cursor, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Get all async");

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(x => x.Tag)
                    .OrderByDescending(x => x.PublishDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<BlogPostListing> GetPublishedAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Get all async (tag = {Tag}, offset = {Offset}, maxResults = {MaxResults})", tag, offset, maxResults);

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                var query = ctx.BlogPosts.Where(x => x.Published);
                var totalPostsQuery = ctx.BlogPosts.Where(x => x.Published);

                if (tag != null)
                {
                    var lowerTag = tag.ToLower();
                    query = query.Where(x => x.BlogPostTags.Any(y => y.TagName.ToLower() == lowerTag));
                    totalPostsQuery = totalPostsQuery.Where(x => x.BlogPostTags.Any(y => y.TagName.ToLower() == lowerTag));
                }

                var totalPosts = await totalPostsQuery.CountAsync(cancellationToken);

                query = query.OrderByDescending(x => x.PublishDate);

                if (offset.HasValue)
                    query = query.Skip(offset.Value);

                if (maxResults.HasValue)
                    query = query.Take(maxResults.Value);

                var posts = await query
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(x => x.Tag)
                    .ToListAsync(cancellationToken);

                return new BlogPostListing { Posts = posts, TotalPosts = totalPosts };
            }
        }

        public async Task<List<BlogPost>> GetFeaturedAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => x.Featured && x.Published)
                    .OrderByDescending(x => x.PublishDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding/updating post: '{Title}' ({ID}) ...", post.Title, post.Id);

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                var existing = await ctx.BlogPosts
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(t => t.Tag)
                    .FirstOrDefaultAsync(x => x.Id == post.Id, cancellationToken);

                if (existing == null)
                {
                    // Update tag references where the tag already exists, so that EF doesn't try to insert a duplicate.

                    var tagList = new List<BlogPostTag>();

                    foreach (var postTag in post.BlogPostTags)
                    {
                        var existingTag = await ctx.Tags.SingleOrDefaultAsync(t => t.Name.ToLower() == postTag.Tag.Name.ToLower(), cancellationToken);
                        postTag.Tag = existingTag ?? postTag.Tag;
                        tagList.Add(postTag);
                    }

                    post.BlogPostTags = tagList;

                    await ctx.BlogPosts.AddAsync(post, cancellationToken);
                }
                else
                {
                    existing.UpdateFrom(post);

                    await UpdateTagsAsync(ctx, existing, post.BlogPostTags.Select(x => x.Tag.Name).ToList());
                }

                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task UpdateTagsAsync(DataContext ctx, BlogPost post, IReadOnlyCollection<string> expectedTags)
        {
            // ReSharper disable SimplifyLinqExpression
            var newTags = expectedTags.Where(et => !post.BlogPostTags.Any(t => t.Tag.Name == et)).ToList();
            var tagsToRemove = post.BlogPostTags.Where(t => !expectedTags.Any(et => et == t.Tag.Name)).ToArray();
            // ReSharper restore SimplifyLinqExpression

            foreach (var newTag in newTags)
            {
                var tag = await ctx.Tags.SingleOrDefaultAsync(t => t.Name == newTag) ?? new Tag(newTag);

                post.BlogPostTags.Add(new BlogPostTag(post, tag));
            }

            foreach (var tagToRemove in tagsToRemove)
            {
                post.BlogPostTags.Remove(tagToRemove);
            }
        }

        public async Task DeleteAsync(IEnumerable<BlogPost> postsToDelete, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                ctx.BlogPosts.RemoveRange(postsToDelete);
                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<BlogPost>> GetRecentAsync(int numRecent, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => x.Published)
                    .OrderByDescending(x => x.PublishDate)
                    .Take(numRecent)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<BlogPost> GetDraftByIdAsync(Guid draftId, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => !x.Published && x.Id == draftId)
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(x => x.Tag)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<BlogPost> GetPublishedByRouteAsync(string route, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => x.Published && x.Route == $"/{route}")
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(x => x.Tag)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPostTags
                                .Where(x => x.BlogPost.Published)
                                .GroupBy(x => x.Tag.Name)
                                .OrderByDescending(x => x.Count())
                                .Select(x => new TagCount(x.Key, x.Count()))
                                .ToListAsync(cancellationToken);
            }
        }

        public async Task RemoveUnusedTagsAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                ctx.Tags.RemoveRange(await ctx.Tags.Where(x => !x.BlogPostTags.Any()).ToListAsync(cancellationToken));
                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetDropboxCursorAsync(string cursor, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Setting dropbox cursor to {Cursor}", cursor);

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                var entity = await ctx.DropboxCursors.SingleOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    entity = new DropboxCursor {Id = Guid.NewGuid()};
                    await ctx.DropboxCursors.AddAsync(entity, cancellationToken);
                }

                entity.Cursor = cursor;

                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<string> GetDropboxCursorAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.DropboxCursors
                                .Select(x => x.Cursor)
                                .SingleOrDefaultAsync(cancellationToken);
            }
        }

        public async Task UpdateDatabaseAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                await ctx.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}