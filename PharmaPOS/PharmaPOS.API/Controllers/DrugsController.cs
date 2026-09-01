using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Drug;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DrugsController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public DrugsController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var drugs = await _context.Drugs
            .Include(d => d.Supplier)
            .Select(d => new DrugDto
            {
                Id = d.Id,
                Name = d.Name,
                GenericName = d.GenericName,
                Barcode = d.Barcode,
                SKU = d.SKU,
                Category = d.Category.ToString(),
                Form = d.Form.ToString(),
                Strength = d.Strength,
                Manufacturer = d.Manufacturer,
                RequiresPrescription = d.RequiresPrescription,
                IsControlled = d.IsControlled,
                CostPrice = d.CostPrice,
                SellingPrice = d.SellingPrice,
                DiscountPercentage = d.DiscountPercentage,
                ReorderLevel = d.ReorderLevel,
                ReorderQuantity = d.ReorderQuantity,
                IsActive = d.IsActive,
                SupplierName = d.Supplier != null ? d.Supplier.Name : null,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        return Ok(drugs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var drug = await _context.Drugs
            .Include(d => d.Supplier)
            .Where(d => d.Id == id)
            .Select(d => new DrugDto
            {
                Id = d.Id,
                Name = d.Name,
                GenericName = d.GenericName,
                Barcode = d.Barcode,
                SKU = d.SKU,
                Category = d.Category.ToString(),
                Form = d.Form.ToString(),
                Strength = d.Strength,
                Manufacturer = d.Manufacturer,
                RequiresPrescription = d.RequiresPrescription,
                IsControlled = d.IsControlled,
                CostPrice = d.CostPrice,
                SellingPrice = d.SellingPrice,
                DiscountPercentage = d.DiscountPercentage,
                ReorderLevel = d.ReorderLevel,
                ReorderQuantity = d.ReorderQuantity,
                IsActive = d.IsActive,
                SupplierName = d.Supplier != null ? d.Supplier.Name : null,
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (drug is null)
            return NotFound(new { message = $"Drug with ID {id} not found." });

        return Ok(drug);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var drug = await _context.Drugs
            .Include(d => d.Supplier)
            .Where(d => d.Barcode == barcode)
            .Select(d => new DrugDto
            {
                Id = d.Id,
                Name = d.Name,
                GenericName = d.GenericName,
                Barcode = d.Barcode,
                SKU = d.SKU,
                Category = d.Category.ToString(),
                Form = d.Form.ToString(),
                Strength = d.Strength,
                Manufacturer = d.Manufacturer,
                RequiresPrescription = d.RequiresPrescription,
                IsControlled = d.IsControlled,
                CostPrice = d.CostPrice,
                SellingPrice = d.SellingPrice,
                DiscountPercentage = d.DiscountPercentage,
                ReorderLevel = d.ReorderLevel,
                ReorderQuantity = d.ReorderQuantity,
                IsActive = d.IsActive,
                SupplierName = d.Supplier != null ? d.Supplier.Name : null,
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (drug is null)
            return NotFound(new { message = $"Drug with barcode {barcode} not found." });

        return Ok(drug);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Create([FromBody] CreateDrugDto dto)
    {
        if (await _context.Drugs.AnyAsync(d => d.Barcode == dto.Barcode))
            return Conflict(new { message = "A drug with this barcode already exists." });

        if (!Enum.TryParse<DrugCategory>(dto.Category, true, out var category))
            return BadRequest(new { message = "Invalid drug category." });

        if (!Enum.TryParse<DrugForm>(dto.Form, true, out var form))
            return BadRequest(new { message = "Invalid drug form." });

        var drug = new Drug
        {
            Name = dto.Name,
            GenericName = dto.GenericName,
            Barcode = dto.Barcode,
            SKU = dto.SKU,
            Category = category,
            Form = form,
            Strength = dto.Strength,
            Manufacturer = dto.Manufacturer,
            Description = dto.Description,
            StorageConditions = dto.StorageConditions,
            RequiresPrescription = dto.RequiresPrescription,
            IsControlled = dto.IsControlled,
            CostPrice = dto.CostPrice,
            SellingPrice = dto.SellingPrice,
            DiscountPercentage = dto.DiscountPercentage,
            ReorderLevel = dto.ReorderLevel,
            ReorderQuantity = dto.ReorderQuantity,
            SupplierId = dto.SupplierId
        };

        await _context.Drugs.AddAsync(drug);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = drug.Id }, new { id = drug.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDrugDto dto)
    {
        var drug = await _context.Drugs.FindAsync(id);

        if (drug is null)
            return NotFound(new { message = $"Drug with ID {id} not found." });

        if (!Enum.TryParse<DrugCategory>(dto.Category, true, out var category))
            return BadRequest(new { message = "Invalid drug category." });

        if (!Enum.TryParse<DrugForm>(dto.Form, true, out var form))
            return BadRequest(new { message = "Invalid drug form." });

        drug.Name = dto.Name;
        drug.GenericName = dto.GenericName;
        drug.SKU = dto.SKU;
        drug.Category = category;
        drug.Form = form;
        drug.Strength = dto.Strength;
        drug.Manufacturer = dto.Manufacturer;
        drug.Description = dto.Description;
        drug.StorageConditions = dto.StorageConditions;
        drug.RequiresPrescription = dto.RequiresPrescription;
        drug.IsControlled = dto.IsControlled;
        drug.CostPrice = dto.CostPrice;
        drug.SellingPrice = dto.SellingPrice;
        drug.DiscountPercentage = dto.DiscountPercentage;
        drug.ReorderLevel = dto.ReorderLevel;
        drug.ReorderQuantity = dto.ReorderQuantity;
        drug.IsActive = dto.IsActive;
        drug.SupplierId = dto.SupplierId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var drug = await _context.Drugs.FindAsync(id);

        if (drug is null)
            return NotFound(new { message = $"Drug with ID {id} not found." });

        drug.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}