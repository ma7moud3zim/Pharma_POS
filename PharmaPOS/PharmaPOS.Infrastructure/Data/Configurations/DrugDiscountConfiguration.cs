using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class DrugDiscountConfiguration : IEntityTypeConfiguration<DrugDiscount>
{
    public void Configure(EntityTypeBuilder<DrugDiscount> builder)
    {
        builder.ToTable("DrugDiscounts");

        builder.HasKey(dd => dd.Id);

        builder.HasIndex(dd => new { dd.DrugId, dd.DiscountId })
            .IsUnique()
            .HasDatabaseName("IX_DrugDiscounts_DrugId_DiscountId");

        builder.HasOne(dd => dd.Drug)
            .WithMany(d => d.DrugDiscounts)
            .HasForeignKey(dd => dd.DrugId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dd => dd.Discount)
            .WithMany(d => d.DrugDiscounts)
            .HasForeignKey(dd => dd.DiscountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}