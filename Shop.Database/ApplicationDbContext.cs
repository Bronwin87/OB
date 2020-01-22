using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Cart;
using Shop.Domain.Models.Orders;
using Shop.Domain.Models.PendingOrders;
using Shop.Domain.Models.Products;
using Shop.Domain.Models.Quotes;
using Shop.Domain.Models.Users;

namespace Shop.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductLink> ProductLinks { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<TertiaryCategory> TertiaryCategories { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountUser> AccountUsers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationAuth> LocationAuth { get; set; } // Added this testing something
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<CostCenterAuth> CostCenterAuth { get; set; }
        public DbSet<Reference> References { get; set; }

        public DbSet<FavouriteList> FavouriteLists { get; set; }
        public DbSet<FavouriteListProduct> FavouriteListProducts { get; set; }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteProduct> QuoteProducts { get; set; }

        public DbSet<PendingOrder> PendingOrders { get; set; }
        public DbSet<PendingOrderProduct> PendingOrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderProduct>()
                .HasKey(x => new { x.ProductId, x.OrderId });

            modelBuilder.Entity<AccountUser>()
                .HasKey(x => new { x.AccountId, x.UserId });

            modelBuilder.Entity<CartProduct>()
                .HasKey(x => new { x.ProductId, x.CartId });

            modelBuilder.Entity<Discount>()
                .HasKey(x => new { x.AccountId, x.ProductId });

            modelBuilder.Entity<FavouriteListProduct>()
                .HasKey(x => new { x.FavouriteListId, x.ProductId });

            modelBuilder.Entity<QuoteProduct>()
                .HasKey(x => new { x.QuoteId, x.ProductId });

            modelBuilder.Entity<PendingOrderProduct>()
                .HasKey(x => new { x.PendingOrderId, x.ProductId });

            modelBuilder.Entity<ProductLink>()
                .HasKey(x => new { x.RootId, x.TargetId });

            modelBuilder.Entity<ProductLink>()
                .HasOne(x => x.Root)
                .WithMany(x => x.Links)
                .HasForeignKey(x => x.RootId);
        }
    }
}
