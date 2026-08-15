using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class Patient : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? KnownAllergies { get; set; }
    public string? ChronicConditions { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;

    // Navigation Properties
    public ICollection<Sale> Sales { get; set; } = [];
    public ICollection<Prescription> Prescriptions { get; set; } = [];
}