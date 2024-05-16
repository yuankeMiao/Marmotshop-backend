using System.Net.NetworkInformation;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Database
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        #region Properties
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        #endregion

        #region Constructors
        static AppDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public AppDbContext(DbContextOptions options, IConfiguration config) : base(options)
        {
            _configuration = config;
        }
        #endregion

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .AddInterceptors(new TimeStampInterceptor())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseSnakeCaseNamingConvention();
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // convert id type
            ConvertIdPropertiesToUUID(modelBuilder);

            // Enums
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OrderStatus>();

            base.OnModelCreating(modelBuilder);

            // Enum columns
            modelBuilder.Entity<User>(entity => entity.Property(u => u.Role).HasColumnType("user_role"));
            modelBuilder.Entity<Order>(entity => entity.Property(o => o.Status).HasColumnType("order_status"));

            // Automatically set CreatedDate when creating data
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>()
               .Property(u => u.UpdatedDate)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>()
               .ToTable("users", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));

            modelBuilder.Entity<Address>()
                .Property(a => a.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Address>()
               .Property(a => a.UpdatedDate)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Address>()
               .ToTable("addresses", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Product>()
                .Property(p => p.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Product>()
               .ToTable("products", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Category>()
                .Property(c => c.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Category>()
               .ToTable("categories", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));

            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>()
                .Property(o => o.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>()
               .ToTable("orders", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));

            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Review>()
                .Property(r => r.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Review>()
               .ToTable("reviews", t => t.HasCheckConstraint("updated_date_check", "updated_date >= created_date "));


            // Constraints for Category
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

            // Constraints for User
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Constraints for product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .ToTable("products", t => t.HasCheckConstraint("product_price_check", "price > 0"));

            modelBuilder.Entity<Product>().HasIndex(p => p.Title).IsUnique();

            // Constraints for Image
            modelBuilder.Entity<Image>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Constraints for Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Constraints for OrderProduct
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.Products)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // OCnstraints for Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // Constraints for Address
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Fetch seed data
            SeedData(modelBuilder);
        }

        #endregion

        #region Helper Methods
        private void ConvertIdPropertiesToUUID(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var idProperty = entityType.FindProperty("Id");
                if (idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.SetColumnType("uuid");
                }
            }
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var categories = SeedingData.GetCategories();
            modelBuilder.Entity<Category>().HasData(categories);

            var users = SeedingData.GetUsers();
            modelBuilder.Entity<User>().HasData(users);

            var products = SeedingData.GetProducts(categories);
            modelBuilder.Entity<Product>().HasData(products);

            var images = SeedingData.GetImages(products);
            modelBuilder.Entity<Image>().HasData(images);

            var addresses = SeedingData.GetAddresses(users);
            modelBuilder.Entity<Address>().HasData(addresses);

            var orders = SeedingData.GetOrders(users);
            modelBuilder.Entity<Order>().HasData(orders);

            var orderProducts = SeedingData.GetOrderProducts(orders, products);
            modelBuilder.Entity<OrderProduct>().HasData(orderProducts);

            var reviews = SeedingData.GetReviews(users, products);
            modelBuilder.Entity<Review>().HasData(reviews);
        }
        #endregion
    }
}