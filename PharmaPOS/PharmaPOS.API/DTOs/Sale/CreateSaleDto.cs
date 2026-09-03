namespace PharmaPOS.API.DTOs.Sale;

public class CreateSaleDto
{
    public Guid? PatientId { get; set; }
    public Guid? PrescriptionId { get; set; }
    public Guid? DiscountId { get; set; }
    public string? Notes { get; set; }
    public string? InsuranceClaimNumber { get; set; }
    public decimal? InsuranceCoveredAmount { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = [];
    public List<CreateSalePaymentDto> Payments { get; set; } = [];
}

public class CreateSaleItemDto
{
    public Guid DrugId { get; set; }
    public Guid? DrugBatchId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public string? DispensingNotes { get; set; }
}

public class CreateSalePaymentDto
{
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}