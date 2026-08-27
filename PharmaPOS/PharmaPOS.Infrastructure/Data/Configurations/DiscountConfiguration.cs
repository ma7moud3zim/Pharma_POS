using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Code)
            .HasMaxLength(50);

        builder.Property(d => d.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.Value)
            .HasColumnType("decimal(18,4)");

        builder.Property(d => d.MinimumPurchaseAmount)
            .HasColumnType("decimal(18,4)");

        

        builder.HasIndex(d => d.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL")
            .HasDatabaseName("IX_Discounts_Code");

        builder.HasMany(d => d.Sales)
            .WithOne(s => s.Discount)
            .HasForeignKey(s => s.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}