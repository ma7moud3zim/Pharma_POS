using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class DrugConfiguration : IEntityTypeConfiguration<Drug>
{
    public void Configure(EntityTypeBuilder<Drug> builder)
    {
        builder.ToTable("Drugs");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.GenericName)
            .HasMaxLength(200);

        builder.Property(d => d.Barcode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.SKU)
            .HasMaxLength(50);

        builder.Property(d => d.Strength)
            .HasMaxLength(50);

        builder.Property(d => d.Manufacturer)
            .HasMaxLength(150);

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.StorageConditions)
            .HasMaxLength(300);

        builder.Property(d => d.CostPrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(d => d.SellingPrice)
            .HasColumnType("decimal(18,4)");

        builder.Property(d => d.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(d => d.Category)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.Form)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(d => d.Barcode)
            .IsUnique()
            .HasDatabaseName("IX_Drugs_Barcode");

        builder.HasIndex(d => d.SKU)
            .IsUnique()
            .HasFilter("[SKU] IS NOT NULL")
            .HasDatabaseName("IX_Drugs_SKU");

        builder.HasIndex(d => d.Name)
            .HasDatabaseName("IX_Drugs_Name");

        builder.HasOne(d => d.Supplier)
            .WithMany(s => s.Drugs)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}