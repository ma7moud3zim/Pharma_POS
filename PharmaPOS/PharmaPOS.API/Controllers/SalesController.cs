using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Sale;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public SalesController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _context.Sales
            .Include(s => s.Patient)
            .Include(s => s.Cashier)
            .Include(s => s.Prescription)
            .Include(s => s.Items)
                .ThenInclude(i => i.Drug)
            .Include(s => s.Items)
                .ThenInclude(i => i.DrugBatch)
            .Include(s => s.Payments)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(s => s.SaleDate >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.SaleDate <= to.Value);

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                PatientName = s.Patient != null ? s.Patient.FullName : null,
                CashierName = s.Cashier.FullName,
                RxNumber = s.Prescription != null ? s.Prescription.RxNumber : null,
                SubTotal = s.SubTotal,
                DiscountAmount = s.DiscountAmount,
                TaxAmount = s.TaxAmount,
                TotalAmount = s.TotalAmount,
                AmountPaid = s.AmountPaid,
                ChangeDue = s.ChangeDue,
                PaymentMethod = s.PaymentMethod.ToString(),
                Status = s.Status.ToString(),
                InsuranceClaimNumber = s.InsuranceClaimNumber,
                InsuranceCoveredAmount = s.InsuranceCoveredAmount,
                Notes = s.Notes,
                Items = s.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    BatchNumber = i.DrugBatch != null ? i.DrugBatch.BatchNumber : null,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountPercent = i.DiscountPercent,
                    DiscountAmount = i.DiscountAmount,
                    TaxPercent = i.TaxPercent,
                    TaxAmount = i.TaxAmount,
                    LineTotal = i.LineTotal,
                    DispensingNotes = i.DispensingNotes
                }).ToList(),
                Payments = s.Payments.Select(p => new SalePaymentDto
                {
                    Id = p.Id,
                    Method = p.Method.ToString(),
                    Amount = p.Amount,
                    ReferenceNumber = p.ReferenceNumber,
                    PaidAt = p.PaidAt
                }).ToList(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(sales);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _context.Sales
            .Include(s => s.Patient)
            .Include(s => s.Cashier)
            .Include(s => s.Prescription)
            .Include(s => s.Items)
                .ThenInclude(i => i.Drug)
            .Include(s => s.Items)
                .ThenInclude(i => i.DrugBatch)
            .Include(s => s.Payments)
            .Where(s => s.Id == id)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                PatientName = s.Patient != null ? s.Patient.FullName : null,
                CashierName = s.Cashier.FullName,
                RxNumber = s.Prescription != null ? s.Prescription.RxNumber : null,
                SubTotal = s.SubTotal,
                DiscountAmount = s.DiscountAmount,
                TaxAmount = s.TaxAmount,
                TotalAmount = s.TotalAmount,
                AmountPaid = s.AmountPaid,
                ChangeDue = s.ChangeDue,
                PaymentMethod = s.PaymentMethod.ToString(),
                Status = s.Status.ToString(),
                InsuranceClaimNumber = s.InsuranceClaimNumber,
                InsuranceCoveredAmount = s.InsuranceCoveredAmount,
                Notes = s.Notes,
                Items = s.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    BatchNumber = i.DrugBatch != null ? i.DrugBatch.BatchNumber : null,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountPercent = i.DiscountPercent,
                    DiscountAmount = i.DiscountAmount,
                    TaxPercent = i.TaxPercent,
                    TaxAmount = i.TaxAmount,
                    LineTotal = i.LineTotal,
                    DispensingNotes = i.DispensingNotes
                }).ToList(),
                Payments = s.Payments.Select(p => new SalePaymentDto
                {
                    Id = p.Id,
                    Method = p.Method.ToString(),
                    Amount = p.Amount,
                    ReferenceNumber = p.ReferenceNumber,
                    PaidAt = p.PaidAt
                }).ToList(),
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (sale is null)
            return NotFound(new { message = $"Sale with ID {id} not found." });

        return Ok(sale);
    }

    [HttpGet("invoice/{invoiceNumber}")]
    public async Task<IActionResult> GetByInvoiceNumber(string invoiceNumber)
    {
        var sale = await _context.Sales
            .Include(s => s.Patient)
            .Include(s => s.Cashier)
            .Include(s => s.Items)
                .ThenInclude(i => i.Drug)
            .Include(s => s.Payments)
            .Where(s => s.InvoiceNumber == invoiceNumber)
            .FirstOrDefaultAsync();

        if (sale is null)
            return NotFound(new { message = $"Sale with invoice {invoiceNumber} not found." });

        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        // Get current cashier
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cashier = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);
        if (cashier is null)
            return Unauthorized(new { message = "Cashier not found." });

        // Validate prescription if provided
        if (dto.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions.FindAsync(dto.PrescriptionId.Value);
            if (prescription is null)
                return NotFound(new { message = "Prescription not found." });
            if (prescription.Status != PrescriptionStatus.Verified)
                return BadRequest(new { message = "Prescription must be verified before dispensing." });
        }

        // Validate drugs and stock
        foreach (var item in dto.Items)
        {
            var drug = await _context.Drugs.FindAsync(item.DrugId);
            if (drug is null)
                return NotFound(new { message = $"Drug with ID {item.DrugId} not found." });

            if (drug.RequiresPrescription && !dto.PrescriptionId.HasValue)
                return BadRequest(new { message = $"{drug.Name} requires a prescription." });

            // Check stock
            var availableStock = await _context.DrugBatches
                .Where(b => b.DrugId == item.DrugId && b.QuantityOnHand >= item.Quantity)
                .SumAsync(b => b.QuantityOnHand);

            if (availableStock < item.Quantity)
                return BadRequest(new { message = $"Insufficient stock for {drug.Name}." });
        }

        // Generate invoice number
        var saleCount = await _context.Sales.CountAsync();
        var invoiceNumber = $"INV-{DateTime.UtcNow.Year}-{(saleCount + 1):D5}";

        // Calculate line items
        var saleItems = new List<SaleItem>();
        decimal subTotal = 0;

        foreach (var item in dto.Items)
        {
            var discountAmount = item.UnitPrice * item.Quantity * (item.DiscountPercent / 100);
            var taxableAmount = (item.UnitPrice * item.Quantity) - discountAmount;
            var taxAmount = taxableAmount * (item.TaxPercent / 100);
            var lineTotal = taxableAmount + taxAmount;

            subTotal += lineTotal;

            saleItems.Add(new SaleItem
            {
                DrugId = item.DrugId,
                DrugBatchId = item.DrugBatchId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                DiscountAmount = discountAmount,
                TaxPercent = item.TaxPercent,
                TaxAmount = taxAmount,
                LineTotal = lineTotal,
                DispensingNotes = item.DispensingNotes
            });

            // Deduct stock from batch
            if (item.DrugBatchId.HasValue)
            {
                var batch = await _context.DrugBatches.FindAsync(item.DrugBatchId.Value);
                if (batch != null)
                    batch.QuantityOnHand -= item.Quantity;
            }
        }

        // Apply discount
        decimal discountTotal = 0;
        if (dto.DiscountId.HasValue)
        {
            var discount = await _context.Discounts.FindAsync(dto.DiscountId.Value);
            if (discount != null && discount.IsActive)
            {
                discountTotal = discount.Type == DiscountType.Percentage
                    ? subTotal * (discount.Value / 100)
                    : discount.Value;
                discount.UsageCount++;
            }
        }

        var totalAmount = subTotal - discountTotal;
        var amountPaid = dto.Payments.Sum(p => p.Amount);
        var changeDue = amountPaid - totalAmount;

        // Determine payment method
        if (!Enum.TryParse<PaymentMethod>(dto.Payments.First().Method, true, out var paymentMethod))
            return BadRequest(new { message = "Invalid payment method." });

        var sale = new Sale
        {
            InvoiceNumber = invoiceNumber,
            CashierId = cashier.Id,
            PatientId = dto.PatientId,
            PrescriptionId = dto.PrescriptionId,
            DiscountId = dto.DiscountId,
            SubTotal = subTotal,
            DiscountAmount = discountTotal,
            TaxAmount = saleItems.Sum(i => i.TaxAmount),
            TotalAmount = totalAmount,
            AmountPaid = amountPaid,
            ChangeDue = changeDue,
            PaymentMethod = dto.Payments.Count > 1 ? PaymentMethod.Mixed : paymentMethod,
            Status = SaleStatus.Completed,
            InsuranceClaimNumber = dto.InsuranceClaimNumber,
            InsuranceCoveredAmount = dto.InsuranceCoveredAmount,
            Notes = dto.Notes,
            Items = saleItems,
            Payments = dto.Payments.Select(p => new Payment
            {
                Method = Enum.Parse<PaymentMethod>(p.Method, true),
                Amount = p.Amount,
                ReferenceNumber = p.ReferenceNumber
            }).ToList()
        };

        // Update prescription status to dispensed
        if (dto.PrescriptionId.HasValue)
        {
            var prescription = await _context.Prescriptions.FindAsync(dto.PrescriptionId.Value);
            if (prescription != null)
                prescription.Status = PrescriptionStatus.Dispensed;
        }

        // Update patient loyalty points and total spent
        if (dto.PatientId.HasValue)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId.Value);
            if (patient != null)
            {
                patient.TotalSpent += totalAmount;
                patient.LoyaltyPoints += (int)(totalAmount / 10); // 1 point per 10 EGP
            }
        }

        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, new
        {
            id = sale.Id,
            invoiceNumber = sale.InvoiceNumber,
            totalAmount = sale.TotalAmount,
            changeDue = sale.ChangeDue
        });
    }

    [HttpPatch("{id:guid}/refund")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Refund(Guid id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale is null)
            return NotFound(new { message = $"Sale with ID {id} not found." });

        if (sale.Status != SaleStatus.Completed)
            return BadRequest(new { message = "Only completed sales can be refunded." });

        // Restore stock
        foreach (var item in sale.Items)
        {
            if (item.DrugBatchId.HasValue)
            {
                var batch = await _context.DrugBatches.FindAsync(item.DrugBatchId.Value);
                if (batch != null)
                    batch.QuantityOnHand += item.Quantity;
            }
        }

        sale.Status = SaleStatus.Refunded;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}