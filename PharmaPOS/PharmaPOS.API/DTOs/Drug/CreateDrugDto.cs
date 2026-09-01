namespace PharmaPOS.API.DTOs.Drug;

public class CreateDrugDto
{
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string? Strength { get; set; }
    public string? Manufacturer { get; set; }
    public string? Description { get; set; }
    public string? StorageConditions { get; set; }
    public bool RequiresPrescription { get; set; } = false;
    public bool IsControlled { get; set; } = false;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public int ReorderQuantity { get; set; } = 50;
    public Guid? SupplierId { get; set; }
}