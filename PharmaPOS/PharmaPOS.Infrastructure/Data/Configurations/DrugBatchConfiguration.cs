using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class DrugBatchConfiguration : IEntityTypeConfiguration<DrugBatch>
{
    public void Configure(EntityTypeBuilder<DrugBatch> builder)
    {
        builder.ToTable("DrugBatches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.LotNumber)
            .HasMaxLength(100);

        builder.Property(b => b.CostPrice)
            .HasColumnType("decimal(18,4)");

        // Ignore computed properties — not stored in DB
        builder.Ignore(b => b.IsExpired);
        builder.Ignore(b => b.IsNearExpiry);

        builder.HasIndex(b => new { b.DrugId, b.BatchNumber })
            .IsUnique()
            .HasDatabaseName("IX_DrugBatches_DrugId_BatchNumber");

        builder.HasIndex(b => b.ExpiryDate)
            .HasDatabaseName("IX_DrugBatches_ExpiryDate");

        builder.HasOne(b => b.Drug)
            .WithMany(d => d.Batches)
            .HasForeignKey(b => b.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.PurchaseOrder)
            .WithMany(po => po.ReceivedBatches)
            .HasForeignKey(b => b.PurchaseOrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}