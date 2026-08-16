using PharmaPOS.Domain.Common;

namespace PharmaPOS.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; } = 0;
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
    public string? Notes { get; set; }

    // Foreign Keys
    public Guid PurchaseOrderId { get; set; }
    public Guid DrugId { get; set; }

    // Navigation Properties
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}