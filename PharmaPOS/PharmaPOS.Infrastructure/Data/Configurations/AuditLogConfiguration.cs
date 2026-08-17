using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaPOS.Domain.Entities;

namespace PharmaPOS.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.TableName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.RecordId)
            .HasMaxLength(50);

        builder.Property(a => a.UserId)
            .HasMaxLength(50);

        builder.Property(a => a.UserName)
            .HasMaxLength(150);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.HasIndex(a => new { a.TableName, a.RecordId })
            .HasDatabaseName("IX_AuditLogs_Table_Record");

        builder.HasIndex(a => a.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");
    }
}