namespace PharmaPOS.API.DTOs.PurchaseOrder;

public class UpdatePurchaseOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? InvoiceReference { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? Notes { get; set; }
}