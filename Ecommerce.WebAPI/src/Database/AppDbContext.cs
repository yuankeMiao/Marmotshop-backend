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
        public DbSet<ProductImage> Images { get; set; }
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
            base.OnConfiguring(optionsBuilder);
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
            modelBuilder.Entity<User>(entity => entity.Property(u => u.UserRole).HasColumnType("user_role"));
            modelBuilder.Entity<Order>(entity => entity.Property(o => o.Status).HasColumnType("order_status"));

            // Automatically set CreatedDate when creating data
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductImage>()
                .Property(i => i.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");


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
                .ToTable("Products", t => t.HasCheckConstraint("product_price_check", "price > 0"));

            modelBuilder.Entity<Product>().HasIndex(p => p.Title).IsUnique();

            // Constraints for Image
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany()
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
                .HasKey(op => new { op.OrderId, op.ProductId });

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

        private void SeedData(ModelBuilder modelBuilder)
        {
            var categories = SeedingData.GetCategories();
            modelBuilder.Entity<Category>().HasData(categories);

            var users = SeedingData.GetUsers();
            modelBuilder.Entity<User>().HasData(users);

        }
        #endregion
    }
}