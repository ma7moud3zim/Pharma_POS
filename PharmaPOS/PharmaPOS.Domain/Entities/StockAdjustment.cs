using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class StockAdjustment : BaseEntity
{
    public int QuantityBefore { get; set; }
    public int QuantityAdjusted { get; set; }
    public int QuantityAfter { get; set; }
    public StockAdjustmentReason Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime AdjustedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid DrugId { get; set; }
    public Guid? DrugBatchId { get; set; }
    public Guid AdjustedByUserId { get; set; }

    // Navigation Properties
    public Drug Drug { get; set; } = null!;
    public DrugBatch? DrugBatch { get; set; }
    public User AdjustedByUser { get; set; } = null!;
}