using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class Payment : BaseEntity
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid SaleId { get; set; }

    // Navigation Properties
    public Sale Sale { get; set; } = null!;
}