namespace PharmaPOS.API.DTOs.Patient;

public class CreatePatientDto
{
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? KnownAllergies { get; set; }
    public string? ChronicConditions { get; set; }
}