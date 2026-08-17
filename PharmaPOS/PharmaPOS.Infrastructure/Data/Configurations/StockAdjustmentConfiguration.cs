using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("StockAdjustments");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Reason)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(sa => sa.Notes)
            .HasMaxLength(500);

        builder.HasIndex(sa => sa.DrugId)
            .HasDatabaseName("IX_StockAdjustments_DrugId");

        builder.HasIndex(sa => sa.AdjustedAt)
            .HasDatabaseName("IX_StockAdjustments_AdjustedAt");

        builder.HasOne(sa => sa.Drug)
            .WithMany()
            .HasForeignKey(sa => sa.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sa => sa.DrugBatch)
            .WithMany()
            .HasForeignKey(sa => sa.DrugBatchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(sa => sa.AdjustedByUser)
            .WithMany(u => u.StockAdjustments)
            .HasForeignKey(sa => sa.AdjustedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(sa => !sa.IsDeleted);
    }
}