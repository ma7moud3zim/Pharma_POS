using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class Drug : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public DrugCategory Category { get; set; }
    public DrugForm Form { get; set; }
    public string? Strength { get; set; }
    public string? Manufacturer { get; set; }
    public string? Description { get; set; }
    public string? StorageConditions { get; set; }
    public bool RequiresPrescription { get; set; } = false;
    public bool IsControlled { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Pricing
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }

    // Reorder
    public int ReorderLevel { get; set; } = 10;
    public int ReorderQuantity { get; set; } = 50;

    // Foreign Keys
    public Guid? SupplierId { get; set; }

    // Navigation Properties
    public Supplier? Supplier { get; set; }
    public ICollection<DrugBatch> Batches { get; set; } = [];
    public ICollection<SaleItem> SaleItems { get; set; } = [];
    public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = [];
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = [];
}