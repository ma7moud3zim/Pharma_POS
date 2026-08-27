using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Entities;
using System.Text.Json;

namespace PharmaPOS.Infrastructure.Data;

public class PharmaPOSDbContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    public PharmaPOSDbContext(DbContextOptions<PharmaPOSDbContext> options,
        ICurrentUserService currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Drug> Drugs => Set<Drug>();
    public DbSet<DrugBatch> DrugBatches => Set<DrugBatch>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<DrugDiscount> DrugDiscounts => Set<DrugDiscount>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Auto-discover all IEntityTypeConfiguration<T> in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmaPOSDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var auditEntries = OnBeforeSave();
        var result = await base.SaveChangesAsync(ct);
        await OnAfterSaveAsync(auditEntries);
        return result;
    }

    private List<AuditEntry> OnBeforeSave()
    {
        ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();
        var now = DateTime.UtcNow;
        var userName = _currentUser.UserName;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Auto-stamp BaseEntity fields
            if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = now;
                        baseEntity.CreatedBy = userName;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = now;
                        baseEntity.UpdatedBy = userName;
                        entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                        entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                        break;
                }
            }

            // Skip AuditLog itself to avoid infinite loop
            if (entry.Entity is AuditLog) continue;
            if (entry.State is EntityState.Detached or EntityState.Unchanged) continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                Action = entry.State switch
                {
                    EntityState.Added => "INSERT",
                    EntityState.Deleted => "DELETE",
                    _ => "UPDATE"
                },
                UserId = _currentUser.UserId,
                UserName = userName
            };

            foreach (var prop in entry.Properties)
            {
                if (prop.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(prop);
                    continue;
                }

                string propName = prop.Metadata.Name;

                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propName] = prop.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propName] = prop.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.OldValues[propName] = prop.OriginalValue;
                        break;
                    case EntityState.Modified when prop.IsModified:
                        auditEntry.OldValues[propName] = prop.OriginalValue;
                        auditEntry.NewValues[propName] = prop.CurrentValue;
                        auditEntry.ChangedColumns.Add(propName);
                        break;
                }
            }

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private async Task OnAfterSaveAsync(List<AuditEntry> auditEntries)
    {
        if (!auditEntries.Any()) return;

        foreach (var entry in auditEntries)
        {
            foreach (var prop in entry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                    entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                else
                    entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            AuditLogs.Add(new AuditLog
            {
                TableName = entry.TableName,
                Action = entry.Action,
                RecordId = entry.KeyValues.Values.FirstOrDefault()?.ToString(),
                OldValues = entry.OldValues.Count > 0 ? JsonSerializer.Serialize(entry.OldValues) : null,
                NewValues = entry.NewValues.Count > 0 ? JsonSerializer.Serialize(entry.NewValues) : null,
                ChangedColumns = entry.ChangedColumns.Count > 0 ? string.Join(",", entry.ChangedColumns) : null,
                UserId = entry.UserId,
                UserName = entry.UserName,
                Timestamp = DateTime.UtcNow
            });
        }

        await base.SaveChangesAsync();
    }
}

// Helper class for building audit entries
internal class AuditEntry(EntityEntry entry)
{
    public EntityEntry Entry { get; } = entry;
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<string> ChangedColumns { get; } = [];
    public List<PropertyEntry> TemporaryProperties { get; } = [];
}

// Interface to get current logged-in user (implemented in API layer)
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? IpAddress { get; }
}
