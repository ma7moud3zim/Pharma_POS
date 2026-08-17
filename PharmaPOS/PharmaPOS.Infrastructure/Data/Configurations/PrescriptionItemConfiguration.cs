using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionItems");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.DrugNameAsWritten)
            .HasMaxLength(200);

        builder.Property(pi => pi.Dosage)
            .HasMaxLength(100);

        builder.Property(pi => pi.Duration)
            .HasMaxLength(100);

        builder.Property(pi => pi.Instructions)
            .HasMaxLength(500);

        builder.HasOne(pi => pi.Prescription)
            .WithMany(rx => rx.Items)
            .HasForeignKey(pi => pi.PrescriptionId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(pi => pi.Drug)
            .WithMany(d => d.PrescriptionItems)
            .HasForeignKey(pi => pi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}