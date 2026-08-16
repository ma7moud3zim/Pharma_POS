using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class Discount : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public int UsageCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool AppliesToAllProducts { get; set; } = true;
    public string? ApplicableDrugIds { get; set; }
}