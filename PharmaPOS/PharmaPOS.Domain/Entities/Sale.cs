using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;
using System.Net.ServerSentEvents;

namespace PharmaPOS.Domain.Entities;

public class Sale : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }

    // Financials
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal ChangeDue { get; set; } = 0;

    // Insurance
    public string? InsuranceClaimNumber { get; set; }
    public decimal? InsuranceCoveredAmount { get; set; }

    // Foreign Keys
    public Guid? DiscountId { get; set; }
    public Guid CashierId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? PrescriptionId { get; set; }

    // Navigation Properties
    public User Cashier { get; set; } = null!;
    public Patient? Patient { get; set; }
    public Prescription? Prescription { get; set; }
    public Discount? Discount { get; set; }
    public ICollection<SaleItem> Items { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}