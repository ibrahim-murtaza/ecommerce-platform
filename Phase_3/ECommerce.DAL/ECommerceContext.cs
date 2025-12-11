using Microsoft.EntityFrameworkCore;
using ECommerce.Models;

namespace ECommerce.DAL
{
    public class ECommerceContext : DbContext
    {
        // Constructor that accepts options (needed for Factory/UI injection later)
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        // Alternatively, parameterless constructor for design-time
        public ECommerceContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // IMPORTANT: Replace 'YourStrong!Passw0rd' with your actual Docker SA password
                // TrustServerCertificate=True is REQUIRED for Docker containers
                optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=ECommerceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PARTITIONING CONFIGURATION
            // Your README says Order and OrderItem use Composite Keys for partitioning

            // Order Primary Key (OrderID + OrderDate)
            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.OrderID, o.OrderDate });

            // OrderItem Primary Key (OrderItemID + OrderDate)
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderItemID, oi.OrderDate });

            // Configure Cart table to use triggers
            // This prevents EF from using OUTPUT clause which conflicts with triggers
            modelBuilder.Entity<Cart>()
                .ToTable(tb => tb.HasTrigger("trg_InsteadOfCart_ValidateStock"));

            // Configure OrderItem table to use triggers
            modelBuilder.Entity<OrderItem>()
                .ToTable(tb => tb.HasTrigger("trg_AfterOrderItem_UpdateStock"));

            // Relationships can be configured here if strictly needed, 
            // but EF often infers them from naming conventions.
        }
    }
}