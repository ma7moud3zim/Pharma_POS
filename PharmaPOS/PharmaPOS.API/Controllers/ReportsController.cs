using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Reports;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public ReportsController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet("daily-sales")]
    public async Task<IActionResult> GetDailySales([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        var nextDay = targetDate.AddDays(1);

        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.SaleDate >= targetDate && s.SaleDate < nextDay)
            .ToListAsync();

        var topDrug = await _context.SaleItems
            .Include(i => i.Drug)
            .Where(i => i.Sale.SaleDate >= targetDate && i.Sale.SaleDate < nextDay)
            .GroupBy(i => i.Drug.Name)
            .OrderByDescending(g => g.Sum(i => i.Quantity))
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        var report = new DailySalesReportDto
        {
            Date = targetDate,
            TotalTransactions = sales.Count,
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            TotalDiscount = sales.Sum(s => s.DiscountAmount),
            TotalTax = sales.Sum(s => s.TaxAmount),
            NetRevenue = sales.Sum(s => s.TotalAmount - s.DiscountAmount),
            TopSellingDrug = topDrug ?? "N/A"
        };

        return Ok(report);
    }

    [HttpGet("top-selling-drugs")]
    public async Task<IActionResult> GetTopSellingDrugs(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int top = 10)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        var topDrugs = await _context.SaleItems
            .Include(i => i.Drug)
            .Include(i => i.Sale)
            .Where(i => i.Sale.SaleDate >= fromDate && i.Sale.SaleDate <= toDate)
            .GroupBy(i => new { i.Drug.Name, i.Drug.Barcode })
            .Select(g => new TopSellingDrugDto
            {
                DrugName = g.Key.Name,
                Barcode = g.Key.Barcode,
                TotalQuantitySold = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.LineTotal)
            })
            .OrderByDescending(d => d.TotalQuantitySold)
            .Take(top)
            .ToListAsync();

        return Ok(topDrugs);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockReport()
    {
        var lowStock = await _context.Drugs
            .Include(d => d.Batches)
            .Include(d => d.Supplier)
            .Where(d => d.IsActive)
            .Select(d => new LowStockReportDto
            {
                DrugName = d.Name,
                Barcode = d.Barcode,
                CurrentStock = d.Batches.Sum(b => b.QuantityOnHand),
                ReorderLevel = d.ReorderLevel,
                ReorderQuantity = d.ReorderQuantity,
                SupplierName = d.Supplier != null ? d.Supplier.Name : null
            })
            .Where(d => d.CurrentStock <= d.ReorderLevel)
            .OrderBy(d => d.CurrentStock)
            .ToListAsync();

        return Ok(lowStock);
    }

    [HttpGet("expiry")]
    public async Task<IActionResult> GetExpiryReport([FromQuery] int days = 90)
    {
        var expiryDate = DateTime.UtcNow.AddDays(days);

        var expiring = await _context.DrugBatches
            .Include(b => b.Drug)
            .Where(b => b.ExpiryDate <= expiryDate && b.QuantityOnHand > 0)
            .Select(b => new ExpiryReportDto
            {
                DrugName = b.Drug.Name,
                BatchNumber = b.BatchNumber,
                QuantityOnHand = b.QuantityOnHand,
                ExpiryDate = b.ExpiryDate,
                DaysUntilExpiry = (int)(b.ExpiryDate - DateTime.UtcNow).TotalDays
            })
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();

        return Ok(expiring);
    }

    [HttpGet("patient-summary")]
    public async Task<IActionResult> GetPatientSummary([FromQuery] int top = 10)
    {
        var patients = await _context.Patients
            .Include(p => p.Sales)
            .Select(p => new PatientSalesReportDto
            {
                PatientName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                TotalVisits = p.Sales.Count,
                TotalSpent = p.TotalSpent,
                LoyaltyPoints = p.LoyaltyPoints,
                LastVisit = p.Sales
                    .OrderByDescending(s => s.SaleDate)
                    .Select(s => s.SaleDate)
                    .FirstOrDefault()
            })
            .OrderByDescending(p => p.TotalSpent)
            .Take(top)
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("sales-summary")]
    public async Task<IActionResult> GetSalesSummary(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        var summary = await _context.Sales
            .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate)
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalTransactions = g.Count(),
                TotalRevenue = g.Sum(s => s.TotalAmount),
                TotalDiscount = g.Sum(s => s.DiscountAmount)
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        return Ok(summary);
    }
}