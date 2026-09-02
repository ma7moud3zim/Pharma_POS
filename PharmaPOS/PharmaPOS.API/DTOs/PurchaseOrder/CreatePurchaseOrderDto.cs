namespace PharmaPOS.API.DTOs.PurchaseOrder;

public class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = [];
}

public class CreatePurchaseOrderItemDto
{
    public Guid DrugId { get; set; }
    public int OrderedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }
}