using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;

namespace OrderSystem.Infrastructure.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.IsCover).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Images)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasConversion<string>();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId)
                  .IsRequired();
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .IsRequired();

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .IsRequired();
        });

        builder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(e => e.User)
                  .WithOne(u => u.Cart)
                  .HasForeignKey<Cart>(e => e.UserId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_CartItems_Quantity", "\"Quantity\" >= 1"));

            entity.HasOne(e => e.Cart)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(e => e.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .IsRequired();
        });
    }
}
