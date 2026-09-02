namespace PharmaPOS.API.DTOs.Inventory;

public class CreateDrugBatchDto
{
    public Guid DrugId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int QuantityReceived { get; set; }
    public decimal CostPrice { get; set; }
    public Guid? PurchaseOrderId { get; set; }
}