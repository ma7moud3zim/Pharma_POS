using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(po => po.Id);

        builder.Property(po => po.OrderNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(po => po.SubTotal)
            .HasColumnType("decimal(18,4)");

        builder.Property(po => po.TaxAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(po => po.TotalAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(po => po.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(po => po.Notes)
            .HasMaxLength(1000);

        builder.Property(po => po.InvoiceReference)
            .HasMaxLength(100);

        builder.HasIndex(po => po.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_PurchaseOrders_OrderNumber");

        builder.HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.CreatedByUser)
            .WithMany(u => u.PurchaseOrders)
            .HasForeignKey(po => po.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(po => !po.IsDeleted);
    }
}