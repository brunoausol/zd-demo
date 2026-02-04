using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasColumnName("id");

        builder.Property(order => order.CustomerName)
            .HasColumnName("customer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(order => order.CustomerEmail)
            .HasColumnName("customer_email")
            .HasMaxLength(320);

        builder.Property(order => order.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();
    }
}
