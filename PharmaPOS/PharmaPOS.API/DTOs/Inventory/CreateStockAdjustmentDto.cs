namespace PharmaPOS.API.DTOs.Inventory;

public class CreateStockAdjustmentDto
{
    public Guid DrugId { get; set; }
    public Guid? DrugBatchId { get; set; }
    public int QuantityAdjusted { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}