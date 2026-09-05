using Hangfire;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.Services;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Jobs;

public class PharmacyBackgroundJobs
{
    private readonly PharmaPOSDbContext _context;
    private readonly NotificationService _notificationService;
    private readonly ILogger<PharmacyBackgroundJobs> _logger;

    public PharmacyBackgroundJobs(
        PharmaPOSDbContext context,
        NotificationService notificationService,
        ILogger<PharmacyBackgroundJobs> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ScanExpiringBatchesAsync()
    {
        _logger.LogInformation("Running expiry scan job at {Time}", DateTime.UtcNow);

        var expiringBatches = await _context.DrugBatches
            .Include(b => b.Drug)
            .Where(b => b.ExpiryDate <= DateTime.UtcNow.AddDays(90) && b.QuantityOnHand > 0)
            .ToListAsync();

        foreach (var batch in expiringBatches)
        {
            await _notificationService.SendExpiryAlertAsync(
                batch.Drug.Name,
                batch.BatchNumber,
                batch.ExpiryDate);
        }

        _logger.LogInformation("Expiry scan completed. Found {Count} expiring batches.", expiringBatches.Count);
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ScanLowStockAsync()
    {
        _logger.LogInformation("Running low stock scan job at {Time}", DateTime.UtcNow);

        var lowStockDrugs = await _context.Drugs
            .Include(d => d.Batches)
            .Where(d => d.IsActive)
            .Where(d => d.Batches.Sum(b => b.QuantityOnHand) <= d.ReorderLevel)
            .ToListAsync();

        foreach (var drug in lowStockDrugs)
        {
            var currentStock = drug.Batches.Sum(b => b.QuantityOnHand);
            await _notificationService.SendLowStockAlertAsync(drug.Name, currentStock);
        }

        _logger.LogInformation("Low stock scan completed. Found {Count} low stock drugs.", lowStockDrugs.Count);
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExpireLoyaltyPointsAsync()
    {
        _logger.LogInformation("Running loyalty points expiry job at {Time}", DateTime.UtcNow);

        var oneYearAgo = DateTime.UtcNow.AddYears(-1);

        var inactivePatients = await _context.Patients
            .Where(p => p.LoyaltyPoints > 0 &&
                        !p.Sales.Any(s => s.SaleDate >= oneYearAgo))
            .ToListAsync();

        foreach (var patient in inactivePatients)
        {
            patient.LoyaltyPoints = 0;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Loyalty expiry completed. Reset points for {Count} patients.", inactivePatients.Count);
    }
}