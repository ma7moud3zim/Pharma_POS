namespace PharmaPOS.API.DTOs.Prescription;

public class VerifyPrescriptionDto
{
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
}