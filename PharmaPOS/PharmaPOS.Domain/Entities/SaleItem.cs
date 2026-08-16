using PharmaPOS.Domain.Common;

namespace PharmaPOS.Domain.Entities;

public class SaleItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal LineTotal { get; set; }
    public string? DispensingNotes { get; set; }

    // Foreign Keys
    public Guid SaleId { get; set; }
    public Guid DrugId { get; set; }
    public Guid? DrugBatchId { get; set; }

    // Navigation Properties
    public Sale Sale { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public DrugBatch? DrugBatch { get; set; }
}