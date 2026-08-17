using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");

        builder.HasKey(rx => rx.Id);

        builder.Property(rx => rx.RxNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(rx => rx.DoctorName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(rx => rx.DoctorLicenseNumber)
            .HasMaxLength(50);

        builder.Property(rx => rx.DoctorPhone)
            .HasMaxLength(20);

        builder.Property(rx => rx.ClinicName)
            .HasMaxLength(200);

        builder.Property(rx => rx.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(rx => rx.RejectionReason)
            .HasMaxLength(500);

        builder.Property(rx => rx.ImageUrl)
            .HasMaxLength(500);

        builder.Property(rx => rx.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(rx => rx.RxNumber)
            .IsUnique()
            .HasDatabaseName("IX_Prescriptions_RxNumber");

        builder.HasIndex(rx => rx.PatientId)
            .HasDatabaseName("IX_Prescriptions_PatientId");

        builder.HasOne(rx => rx.Patient)
            .WithMany(p => p.Prescriptions)
            .HasForeignKey(rx => rx.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rx => rx.VerifiedBy)
            .WithMany(u => u.VerifiedPrescriptions)
            .HasForeignKey(rx => rx.VerifiedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(rx => rx.Items)
            .WithOne(i => i.Prescription)
            .HasForeignKey(i => i.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(rx => !rx.IsDeleted);
    }
}