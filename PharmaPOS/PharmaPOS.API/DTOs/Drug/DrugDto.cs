namespace PharmaPOS.API.DTOs.Drug;

public class DrugDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string? Strength { get; set; }
    public string? Manufacturer { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; }
    public string? SupplierName { get; set; }
    public DateTime CreatedAt { get; set; }
}