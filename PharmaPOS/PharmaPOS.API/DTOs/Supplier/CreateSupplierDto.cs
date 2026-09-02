namespace PharmaPOS.API.DTOs.Supplier;

public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermDays { get; set; }
}