using Microsoft.EntityFrameworkCore;
using ECommerce.Models;

namespace ECommerce.DAL
{
    public class ECommerceContext : DbContext
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public ECommerceContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=ECommerceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
                optionsBuilder.UseSqlServer("Server=localhost;Database=ECommerceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
            
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

            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.OrderID, o.OrderDate });

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderItemID, oi.OrderDate });

            modelBuilder.Entity<Cart>()
                .ToTable(tb => tb.HasTrigger("trg_InsteadOfCart_ValidateStock"));

            modelBuilder.Entity<OrderItem>()
                .ToTable(tb => tb.HasTrigger("trg_AfterOrderItem_UpdateStock"));

        }
    }
}