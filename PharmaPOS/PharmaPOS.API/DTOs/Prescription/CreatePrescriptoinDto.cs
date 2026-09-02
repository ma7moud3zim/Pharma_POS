namespace PharmaPOS.API.DTOs.Prescription;

public class CreatePrescriptionDto
{
    public Guid PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorLicenseNumber { get; set; }
    public string? DoctorPhone { get; set; }
    public string? ClinicName { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePrescriptionItemDto> Items { get; set; } = [];
}

public class CreatePrescriptionItemDto
{
    public Guid DrugId { get; set; }
    public string? DrugNameAsWritten { get; set; }
    public int PrescribedQuantity { get; set; }
    public string? Dosage { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public bool IsSubstitutionAllowed { get; set; } = false;
}