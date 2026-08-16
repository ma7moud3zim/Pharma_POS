using PharmaPOS.Domain.Common;

namespace PharmaPOS.Domain.Entities;

public class DrugBatch : BaseEntity
{
    public string BatchNumber { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityOnHand { get; set; }
    public decimal CostPrice { get; set; }

    // Foreign Keys
    public Guid DrugId { get; set; }
    public Guid? PurchaseOrderId { get; set; }

    // Navigation Properties
    public Drug Drug { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }

    // Computed Properties
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsNearExpiry => ExpiryDate < DateTime.UtcNow.AddDays(90) && !IsExpired;
}