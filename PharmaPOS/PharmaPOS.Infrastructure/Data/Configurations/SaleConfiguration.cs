using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.SubTotal)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.DiscountAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.TaxAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.TotalAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.AmountPaid)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.ChangeDue)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.InsuranceCoveredAmount)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.InsuranceClaimNumber)
            .HasMaxLength(100);

        builder.HasIndex(s => s.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_Sales_InvoiceNumber");

        builder.HasIndex(s => s.SaleDate)
            .HasDatabaseName("IX_Sales_SaleDate");

        builder.HasIndex(s => s.PatientId)
            .HasDatabaseName("IX_Sales_PatientId");

        builder.HasOne(s => s.Patient)
            .WithMany(p => p.Sales)
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Cashier)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.CashierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Prescription)
            .WithMany(rx => rx.Sales)
            .HasForeignKey(s => s.PrescriptionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}