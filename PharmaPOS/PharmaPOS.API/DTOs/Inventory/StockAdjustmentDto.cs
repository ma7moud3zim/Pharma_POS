namespace PharmaPOS.API.DTOs.Inventory;

public class StockAdjustmentDto
{
    public Guid Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAdjusted { get; set; }
    public int QuantityAfter { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string AdjustedByUser { get; set; } = string.Empty;
    public DateTime AdjustedAt { get; set; }
}