using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.PurchaseOrder;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public PurchaseOrdersController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.CreatedByUser)
            .Include(po => po.Items)
                .ThenInclude(i => i.Drug)
            .Select(po => new PurchaseOrderDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                SupplierName = po.Supplier.Name,
                CreatedByUser = po.CreatedByUser.FullName,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                ActualDeliveryDate = po.ActualDeliveryDate,
                Status = po.Status.ToString(),
                SubTotal = po.SubTotal,
                TaxAmount = po.TaxAmount,
                TotalAmount = po.TotalAmount,
                Notes = po.Notes,
                InvoiceReference = po.InvoiceReference,
                Items = po.Items.Select(i => new PurchaseOrderItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    OrderedQuantity = i.OrderedQuantity,
                    ReceivedQuantity = i.ReceivedQuantity,
                    UnitCost = i.UnitCost,
                    LineTotal = i.LineTotal,
                    Notes = i.Notes
                }).ToList(),
                CreatedAt = po.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.CreatedByUser)
            .Include(po => po.Items)
                .ThenInclude(i => i.Drug)
            .Where(po => po.Id == id)
            .Select(po => new PurchaseOrderDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                SupplierName = po.Supplier.Name,
                CreatedByUser = po.CreatedByUser.FullName,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                ActualDeliveryDate = po.ActualDeliveryDate,
                Status = po.Status.ToString(),
                SubTotal = po.SubTotal,
                TaxAmount = po.TaxAmount,
                TotalAmount = po.TotalAmount,
                Notes = po.Notes,
                InvoiceReference = po.InvoiceReference,
                Items = po.Items.Select(i => new PurchaseOrderItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    OrderedQuantity = i.OrderedQuantity,
                    ReceivedQuantity = i.ReceivedQuantity,
                    UnitCost = i.UnitCost,
                    LineTotal = i.LineTotal,
                    Notes = i.Notes
                }).ToList(),
                CreatedAt = po.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (order is null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        return Ok(order);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Pharmacist")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
    {
        var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
        if (supplier is null)
            return NotFound(new { message = "Supplier not found." });

        // Get current user
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);
        if (user is null)
            return Unauthorized(new { message = "User not found." });

        // Generate order number
        var orderCount = await _context.PurchaseOrders.CountAsync();
        var orderNumber = $"PO-{DateTime.UtcNow.Year}-{(orderCount + 1):D5}";

        // Calculate totals
        var subTotal = dto.Items.Sum(i => i.OrderedQuantity * i.UnitCost);
        var taxAmount = subTotal * 0.15m; // 15% VAT
        var totalAmount = subTotal + taxAmount;

        var order = new PurchaseOrder
        {
            OrderNumber = orderNumber,
            SupplierId = dto.SupplierId,
            CreatedByUserId = user.Id,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Notes = dto.Notes,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            Status = PurchaseOrderStatus.Draft,
            Items = dto.Items.Select(i => new PurchaseOrderItem
            {
                DrugId = i.DrugId,
                OrderedQuantity = i.OrderedQuantity,
                UnitCost = i.UnitCost,
                LineTotal = i.OrderedQuantity * i.UnitCost,
                Notes = i.Notes
            }).ToList()
        };

        await _context.PurchaseOrders.AddAsync(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, new { id = order.Id });
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePurchaseOrderStatusDto dto)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);
        if (order is null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        if (!Enum.TryParse<PurchaseOrderStatus>(dto.Status, true, out var status))
            return BadRequest(new { message = "Invalid status." });

        order.Status = status;
        order.InvoiceReference = dto.InvoiceReference;
        order.ActualDeliveryDate = dto.ActualDeliveryDate;
        order.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);
        if (order is null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        if (order.Status != PurchaseOrderStatus.Draft)
            return BadRequest(new { message = "Only draft orders can be deleted." });

        order.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}