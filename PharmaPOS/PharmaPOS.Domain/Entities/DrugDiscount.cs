using PharmaPOS.Domain.Common;

namespace PharmaPOS.Domain.Entities;

public class DrugDiscount : BaseEntity
{
    public Guid DrugId { get; set; }
    public Guid DiscountId { get; set; }

    // Navigation Properties
    public Drug Drug { get; set; } = null!;
    public Discount Discount { get; set; } = null!;
}