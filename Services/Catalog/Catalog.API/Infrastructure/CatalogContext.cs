using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure
{
    public class CatalogContext : DbContext, IDesignTimeDbContextFactory<CatalogContext>
    {
        public CatalogContext()
        {
        }

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public CatalogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>();
            optionsBuilder.UseSqlServer("Server=tcp:127.0.0.1,1433;Database=CatalogDb;User Id=sa;Password=P@ssword;");

            return new CatalogContext(optionsBuilder.Options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
