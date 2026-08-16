using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public string? Notes { get; set; }
    public string? InvoiceReference { get; set; }

    // Financials
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; }

    // Foreign Keys
    public Guid SupplierId { get; set; }
    public Guid CreatedByUserId { get; set; }

    // Navigation Properties
    public Supplier Supplier { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
    public ICollection<DrugBatch> ReceivedBatches { get; set; } = [];
}