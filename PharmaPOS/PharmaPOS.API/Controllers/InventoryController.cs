using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Inventory;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public InventoryController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    // ── Drug Batches ──────────────────────────────────────────────────────

    [HttpGet("batches")]
    public async Task<IActionResult> GetAllBatches()
    {
        var batches = await _context.DrugBatches
            .Include(b => b.Drug)
            .Select(b => new DrugBatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                LotNumber = b.LotNumber,
                ManufactureDate = b.ManufactureDate,
                ExpiryDate = b.ExpiryDate,
                QuantityReceived = b.QuantityReceived,
                QuantityOnHand = b.QuantityOnHand,
                CostPrice = b.CostPrice,
                IsExpired = b.IsExpired,
                IsNearExpiry = b.IsNearExpiry,
                DrugName = b.Drug.Name,
                DrugId = b.DrugId,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(batches);
    }

    [HttpGet("batches/{id:guid}")]
    public async Task<IActionResult> GetBatchById(Guid id)
    {
        var batch = await _context.DrugBatches
            .Include(b => b.Drug)
            .Where(b => b.Id == id)
            .Select(b => new DrugBatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                LotNumber = b.LotNumber,
                ManufactureDate = b.ManufactureDate,
                ExpiryDate = b.ExpiryDate,
                QuantityReceived = b.QuantityReceived,
                QuantityOnHand = b.QuantityOnHand,
                CostPrice = b.CostPrice,
                IsExpired = b.IsExpired,
                IsNearExpiry = b.IsNearExpiry,
                DrugName = b.Drug.Name,
                DrugId = b.DrugId,
                CreatedAt = b.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (batch is null)
            return NotFound(new { message = $"Batch with ID {id} not found." });

        return Ok(batch);
    }

    [HttpGet("batches/drug/{drugId:guid}")]
    public async Task<IActionResult> GetBatchesByDrug(Guid drugId)
    {
        var batches = await _context.DrugBatches
            .Include(b => b.Drug)
            .Where(b => b.DrugId == drugId)
            .Select(b => new DrugBatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                LotNumber = b.LotNumber,
                ManufactureDate = b.ManufactureDate,
                ExpiryDate = b.ExpiryDate,
                QuantityReceived = b.QuantityReceived,
                QuantityOnHand = b.QuantityOnHand,
                CostPrice = b.CostPrice,
                IsExpired = b.IsExpired,
                IsNearExpiry = b.IsNearExpiry,
                DrugName = b.Drug.Name,
                DrugId = b.DrugId,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(batches);
    }

    [HttpGet("batches/expiring")]
    public async Task<IActionResult> GetExpiringBatches()
    {
        var ninetyDaysFromNow = DateTime.UtcNow.AddDays(90);

        var batches = await _context.DrugBatches
            .Include(b => b.Drug)
            .Where(b => b.ExpiryDate <= ninetyDaysFromNow && b.QuantityOnHand > 0)
            .Select(b => new DrugBatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                LotNumber = b.LotNumber,
                ManufactureDate = b.ManufactureDate,
                ExpiryDate = b.ExpiryDate,
                QuantityReceived = b.QuantityReceived,
                QuantityOnHand = b.QuantityOnHand,
                CostPrice = b.CostPrice,
                IsExpired = b.IsExpired,
                IsNearExpiry = b.IsNearExpiry,
                DrugName = b.Drug.Name,
                DrugId = b.DrugId,
                CreatedAt = b.CreatedAt
            })
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();

        return Ok(batches);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockDrugs()
    {
        var lowStockDrugs = await _context.Drugs
            .Where(d => d.IsActive)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Barcode,
                d.ReorderLevel,
                d.ReorderQuantity,
                TotalStock = d.Batches.Sum(b => b.QuantityOnHand)
            })
            .Where(d => d.TotalStock <= d.ReorderLevel)
            .ToListAsync();

        return Ok(lowStockDrugs);
    }

    [HttpPost("batches")]
    [Authorize(Roles = "Admin,Pharmacist,Manager")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateDrugBatchDto dto)
    {
        var drug = await _context.Drugs.FindAsync(dto.DrugId);
        if (drug is null)
            return NotFound(new { message = "Drug not found." });

        if (await _context.DrugBatches.AnyAsync(b => b.DrugId == dto.DrugId && b.BatchNumber == dto.BatchNumber))
            return Conflict(new { message = "A batch with this number already exists for this drug." });

        var batch = new DrugBatch
        {
            DrugId = dto.DrugId,
            BatchNumber = dto.BatchNumber,
            LotNumber = dto.LotNumber,
            ManufactureDate = dto.ManufactureDate,
            ExpiryDate = dto.ExpiryDate,
            QuantityReceived = dto.QuantityReceived,
            QuantityOnHand = dto.QuantityReceived,
            CostPrice = dto.CostPrice,
            PurchaseOrderId = dto.PurchaseOrderId
        };

        await _context.DrugBatches.AddAsync(batch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBatchById), new { id = batch.Id }, new { id = batch.Id });
    }

    // ── Stock Adjustments ─────────────────────────────────────────────────

    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAllAdjustments()
    {
        var adjustments = await _context.StockAdjustments
            .Include(a => a.Drug)
            .Include(a => a.DrugBatch)
            .Include(a => a.AdjustedByUser)
            .Select(a => new StockAdjustmentDto
            {
                Id = a.Id,
                DrugName = a.Drug.Name,
                BatchNumber = a.DrugBatch != null ? a.DrugBatch.BatchNumber : null,
                QuantityBefore = a.QuantityBefore,
                QuantityAdjusted = a.QuantityAdjusted,
                QuantityAfter = a.QuantityAfter,
                Reason = a.Reason.ToString(),
                Notes = a.Notes,
                AdjustedByUser = a.AdjustedByUser.FullName,
                AdjustedAt = a.AdjustedAt
            })
            .ToListAsync();

        return Ok(adjustments);
    }

    [HttpPost("adjustments")]
    [Authorize(Roles = "Admin,Pharmacist,Manager")]
    public async Task<IActionResult> CreateAdjustment([FromBody] CreateStockAdjustmentDto dto)
    {
        var drug = await _context.Drugs.FindAsync(dto.DrugId);
        if (drug is null)
            return NotFound(new { message = "Drug not found." });

        if (!Enum.TryParse<StockAdjustmentReason>(dto.Reason, true, out var reason))
            return BadRequest(new { message = "Invalid adjustment reason." });

        DrugBatch? batch = null;
        if (dto.DrugBatchId.HasValue)
        {
            batch = await _context.DrugBatches.FindAsync(dto.DrugBatchId.Value);
            if (batch is null)
                return NotFound(new { message = "Drug batch not found." });
        }

        var currentStock = batch?.QuantityOnHand ??
            await _context.DrugBatches
                .Where(b => b.DrugId == dto.DrugId)
                .SumAsync(b => b.QuantityOnHand);

        var newStock = currentStock + dto.QuantityAdjusted;
        if (newStock < 0)
            return BadRequest(new { message = "Adjustment would result in negative stock." });

        if (batch != null)
            batch.QuantityOnHand = newStock;

        // Get current user ID from claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);

        var adjustment = new StockAdjustment
        {
            DrugId = dto.DrugId,
            DrugBatchId = dto.DrugBatchId,
            AdjustedByUserId = user?.Id ?? Guid.Empty,
            QuantityBefore = currentStock,
            QuantityAdjusted = dto.QuantityAdjusted,
            QuantityAfter = newStock,
            Reason = reason,
            Notes = dto.Notes
        };

        await _context.StockAdjustments.AddAsync(adjustment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllAdjustments), new { id = adjustment.Id }, new { id = adjustment.Id });
    }
}