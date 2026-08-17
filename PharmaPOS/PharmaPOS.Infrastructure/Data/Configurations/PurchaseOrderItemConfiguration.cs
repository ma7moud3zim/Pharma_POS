using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");

        builder.HasKey(poi => poi.Id);

        builder.Property(poi => poi.UnitCost)
            .HasColumnType("decimal(18,4)");

        builder.Property(poi => poi.LineTotal)
            .HasColumnType("decimal(18,4)");

        builder.Property(poi => poi.Notes)
            .HasMaxLength(500);

        builder.HasOne(poi => poi.PurchaseOrder)
            .WithMany(po => po.Items)
            .HasForeignKey(poi => poi.PurchaseOrderId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(poi => poi.Drug)
            .WithMany(d => d.PurchaseOrderItems)
            .HasForeignKey(poi => poi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}