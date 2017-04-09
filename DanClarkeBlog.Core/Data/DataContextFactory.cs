using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DanClarkeBlog.Core.Data
{
    public class DataContextFactory : IDbContextFactory<DataContext>
    {
        public DataContext Create(DbContextFactoryOptions options)
        {
            var connectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString");

            return new DataContext(connectionString);
        }
    }
}
