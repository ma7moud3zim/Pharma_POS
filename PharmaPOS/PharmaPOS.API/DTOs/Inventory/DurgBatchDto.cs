namespace PharmaPOS.API.DTOs.Inventory;

public class DrugBatchDto
{
    public Guid Id { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityOnHand { get; set; }
    public decimal CostPrice { get; set; }
    public bool IsExpired { get; set; }
    public bool IsNearExpiry { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public Guid DrugId { get; set; }
    public DateTime CreatedAt { get; set; }
}