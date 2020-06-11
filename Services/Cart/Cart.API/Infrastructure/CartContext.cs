using Cart.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cart.API.Infrastructure
{
    public class CartContext : DbContext, IDesignTimeDbContextFactory<CartContext>
    {
        public CartContext()
        {
        }
        public CartContext(DbContextOptions<CartContext> options) : base(options)
        {
        }

        public DbSet<Models.Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        /// <summary>
        /// Enable migrations of DB
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CartContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CartContext>();
            optionsBuilder.UseSqlServer("Server=tcp:127.0.0.1,1433;Database=CartDb;User Id=sa;Password=P@ssword;");

            return new CartContext(optionsBuilder.Options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
