using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Supplier;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public SuppliersController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _context.Suppliers
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Address = s.Address,
                TaxNumber = s.TaxNumber,
                LicenseNumber = s.LicenseNumber,
                CreditLimit = s.CreditLimit,
                PaymentTermDays = s.PaymentTermDays,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == id)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Address = s.Address,
                TaxNumber = s.TaxNumber,
                LicenseNumber = s.LicenseNumber,
                CreditLimit = s.CreditLimit,
                PaymentTermDays = s.PaymentTermDays,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (supplier is null)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        return Ok(supplier);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactPerson = dto.ContactPerson,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Address = dto.Address,
            TaxNumber = dto.TaxNumber,
            LicenseNumber = dto.LicenseNumber,
            CreditLimit = dto.CreditLimit,
            PaymentTermDays = dto.PaymentTermDays
        };

        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, new { id = supplier.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierDto dto)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier is null)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        supplier.Name = dto.Name;
        supplier.ContactPerson = dto.ContactPerson;
        supplier.PhoneNumber = dto.PhoneNumber;
        supplier.Email = dto.Email;
        supplier.Address = dto.Address;
        supplier.TaxNumber = dto.TaxNumber;
        supplier.LicenseNumber = dto.LicenseNumber;
        supplier.CreditLimit = dto.CreditLimit;
        supplier.PaymentTermDays = dto.PaymentTermDays;
        supplier.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier is null)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        supplier.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}