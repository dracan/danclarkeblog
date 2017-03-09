using DanClarkeBlog.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DanClarkeBlog.Core.Data
{
    public class DataContext : DbContext
    {
        private readonly Settings _settings;

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DataContext(Settings settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_settings.BlogSqlConnectionString);
        }
    }
}
