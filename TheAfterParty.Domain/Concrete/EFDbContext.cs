using System.Data.Entity;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<StockedProduct> StockedProducts { get; set; }
        public DbSet<DiscountedProduct> DiscountedProducts { get; set; }
        public DbSet<ProductKey> ProductKeys { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<Objective> Objectives { get; set; }
    }
}
