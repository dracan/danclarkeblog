using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Data;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DanClarkeBlog.Core.Repositories
{
    public class BlogPostSqlServerRepository : IBlogPostRepository
    {
        private readonly Settings _setting;
        private readonly ILogger _logger;

        public BlogPostSqlServerRepository(Settings setting, ILogger logger)
        {
            _setting = setting;
            _logger = logger;
        }

        public void CreateDatabase()
        {
            _logger.Trace("CreateDatabase");

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                ctx.Database.EnsureCreated();
            }
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.Trace("Get all async");

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => x.Published)
                    .OrderByDescending(x => x.PublishDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<BlogPostListing> GetAllAsync(string tag, int? offset, int? maxResults, CancellationToken cancellationToken)
        {
            _logger.Trace($"Get all async (tag = {tag}, offset = {offset}, maxResults = {maxResults})");

            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                var totalPosts = await ctx.BlogPosts.CountAsync(cancellationToken);

                var query = ctx.BlogPosts.Where(x => x.Published);

                if (tag != null)
                {
                    var lowerTag = tag.ToLower();
                    query = query.Where(x => x.BlogPostTags.Any(y => y.Tag.Name == lowerTag));
                }

                query = query
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(x => x.Tag)
                    .AsQueryable();

                if (offset.HasValue)
                {
                    query = query.Skip(offset.Value);
                }

                if (maxResults.HasValue)
                {
                    query = query.Take(maxResults.Value);
                }

                return new BlogPostListing
                       {
                           Posts = await query.OrderByDescending(x => x.PublishDate).ToListAsync(cancellationToken),
                           TotalPosts = totalPosts
                       };
            }
        }

        public Task<IEnumerable<BlogPost>> GetUpdatesAsync(CursorContainer cursor, CancellationToken cancellationToken)
        {
            _logger.Error("Trying to call unsupported GetUpdatesAsync on SQL Server implementation");
            throw new NotSupportedException();
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

        public async Task<IEnumerable<BlogPost>> GetWithConditionAsync(Func<BlogPost, bool> conditionFunc, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPosts
                    .Where(x => conditionFunc(x))
                    .OrderByDescending(x => x.PublishDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task AddAsync(BlogPost post, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                await ctx.BlogPosts.AddAsync(post, cancellationToken);
                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task AddOrUpdateAsync(BlogPost post, CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                var existing = await ctx.BlogPosts
                    .Include(x => x.BlogPostTags)
                    .ThenInclude(t => t.Tag)
                    .FirstOrDefaultAsync(x => x.Title == post.Title, cancellationToken);

                if (existing == null)
                {
                    await ctx.BlogPosts.AddAsync(post, cancellationToken);
                }
                else
                {
                    existing.UpdateFrom(post);

                    await UpdateTagsAsync(ctx, post, existing.BlogPostTags.Select(x => x.Tag.Name).ToList());
                }

                await ctx.SaveChangesAsync(cancellationToken);
            }
        }

        private Task UpdateTagsAsync(DataContext ctx, BlogPost post, IReadOnlyCollection<string> expectedTags)
        {
            // ReSharper disable SimplifyLinqExpression
            var newTags = expectedTags.Where(et => !post.BlogPostTags.Any(t => t.Tag.Name == et));
            var tagsToRemove = post.BlogPostTags.Where(t => !expectedTags.Any(et => et == t.Tag.Name));
            // ReSharper restore SimplifyLinqExpression

            foreach (var newTag in newTags)
            {
                var tag = ctx.Tags.SingleOrDefault(t => t.Name == newTag) ?? new Tag(newTag);

                post.BlogPostTags.Add(new BlogPostTag(post, tag));
            }

            foreach (var tagToRemove in tagsToRemove)
            {
                post.BlogPostTags.Remove(new BlogPostTag(post, tagToRemove.Tag));
            }

            return Task.FromResult(true);
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
                    .OrderByDescending(x => x.PublishDate)
                    .Take(numRecent)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<List<TagCount>> GetTagCountsAsync(CancellationToken cancellationToken)
        {
            using (var ctx = new DataContext(_setting.BlogSqlConnectionString))
            {
                return await ctx.BlogPostTags
                                .GroupBy(x => x.Tag.Name)
                                .OrderByDescending(x => x.Count())
                                .Select(x => new TagCount(x.Key, x.Count()))
                                .ToListAsync(cancellationToken);
            }
        }

        public async Task SetDropboxCursorAsync(string cursor, CancellationToken cancellationToken)
        {
            _logger.Trace($"Setting dropbox cursor to {cursor}");

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