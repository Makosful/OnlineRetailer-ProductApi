using Microsoft.EntityFrameworkCore;
using ProductApi.Entities;

namespace ProductApi.Data.Database
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ProductOutput> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductOutput>().HasKey(x => x.Id);
            modelBuilder.Entity<ProductOutput>().HasIndex(x => x.Name).IsUnique();
            
            modelBuilder.Entity<ProductOutput>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<ProductOutput>().Property(x => x.Price).IsRequired();
            modelBuilder.Entity<ProductOutput>().Property(x => x.ItemsReserved).IsRequired();
            modelBuilder.Entity<ProductOutput>().Property(x => x.ItemsInStock).IsRequired();
        }
    }
}