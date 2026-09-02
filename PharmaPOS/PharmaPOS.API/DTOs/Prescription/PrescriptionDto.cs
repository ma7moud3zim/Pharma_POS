namespace PharmaPOS.API.DTOs.Prescription;

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public string RxNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorLicenseNumber { get; set; }
    public string? DoctorPhone { get; set; }
    public string? ClinicName { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? Notes { get; set; }
    public List<PrescriptionItemDto> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class PrescriptionItemDto
{
    public Guid Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? DrugNameAsWritten { get; set; }
    public int PrescribedQuantity { get; set; }
    public int? DispensedQuantity { get; set; }
    public string? Dosage { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public bool IsSubstitutionAllowed { get; set; }
}