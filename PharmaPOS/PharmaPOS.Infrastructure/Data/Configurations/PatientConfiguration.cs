using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(200);

        builder.Property(p => p.NationalId)
            .HasMaxLength(50);

        builder.Property(p => p.InsuranceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.InsuranceProvider)
            .HasMaxLength(150);

        builder.Property(p => p.KnownAllergies)
            .HasMaxLength(1000);

        builder.Property(p => p.ChronicConditions)
            .HasMaxLength(1000);

        builder.Property(p => p.TotalSpent)
            .HasColumnType("decimal(18,4)");

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.HasIndex(p => p.PhoneNumber)
            .HasDatabaseName("IX_Patients_Phone");

        builder.HasIndex(p => p.NationalId)
            .IsUnique()
            .HasFilter("[NationalId] IS NOT NULL")
            .HasDatabaseName("IX_Patients_NationalId");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}