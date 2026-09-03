namespace PharmaPOS.API.DTOs.Sale;

public class SaleDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string? PatientName { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public string? RxNumber { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal ChangeDue { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? InsuranceClaimNumber { get; set; }
    public decimal? InsuranceCoveredAmount { get; set; }
    public string? Notes { get; set; }
    public List<SaleItemDto> Items { get; set; } = [];
    public List<SalePaymentDto> Payments { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class SaleItemDto
{
    public Guid Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public string? DispensingNotes { get; set; }
}

public class SalePaymentDto
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime PaidAt { get; set; }
}