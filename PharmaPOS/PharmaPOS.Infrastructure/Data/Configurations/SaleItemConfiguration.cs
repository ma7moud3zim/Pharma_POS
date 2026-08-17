using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(si => si.DiscountPercent)
            .HasColumnType("decimal(5,2)");

        builder.Property(si => si.DiscountAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(si => si.TaxPercent)
            .HasColumnType("decimal(5,2)");

        builder.Property(si => si.TaxAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(si => si.LineTotal)
            .HasColumnType("decimal(18,4)");

        builder.Property(si => si.DispensingNotes)
            .HasMaxLength(500);

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(si => si.Drug)
            .WithMany(d => d.SaleItems)
            .HasForeignKey(si => si.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(si => si.DrugBatch)
            .WithMany()
            .HasForeignKey(si => si.DrugBatchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}