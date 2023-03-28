using EfCoreVsDapper.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace EfCoreVsDapper.Database
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDbConnection _dbConnection;

        public ApplicationDbContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite((DbConnection)_dbConnection);
        }
    }
}
