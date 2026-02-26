using Microsoft.EntityFrameworkCore;
using Orders.Abp.Domain.Entities;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Orders.Abp.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class OrdersAbpDbContext : AbpDbContext<OrdersAbpDbContext>
{
    public DbSet<Order> Orders => Set<Order>();

    public OrdersAbpDbContext(DbContextOptions<OrdersAbpDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>(b =>
        {
            b.ToTable("orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.CustomerName)
                .HasColumnName("customer_name")
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.TotalAmount)
                .HasColumnName("total_amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            b.Property(x => x.CreationTime)
                .HasColumnName("created_at_utc")
                .IsRequired();
        });
    }
}
