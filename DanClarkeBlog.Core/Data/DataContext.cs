using DanClarkeBlog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DanClarkeBlog.Core.Data
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<DropboxCursor> DropboxCursors { get; set; }

        public DataContext(Settings settings)
        {
            _connectionString = settings.AzureStorageConnectionString;
        }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BlogPostTag>()
                   .HasKey(t => new {t.BlogPostId, t.TagId});

            builder.Entity<BlogPostTag>()
                   .HasOne(t => t.BlogPost)
                   .WithMany(t => t.BlogPostTags)
                   .HasForeignKey(t => t.BlogPostId);

            builder.Entity<BlogPostTag>()
                   .HasOne(t => t.Tag)
                   .WithMany(t => t.BlogPostTags)
                   .HasForeignKey(t => t.TagId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
