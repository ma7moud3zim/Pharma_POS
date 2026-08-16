using PharmaPOS.Domain.Common;
using PharmaPOS.Domain.Enums;

namespace PharmaPOS.Domain.Entities;

public class Prescription : BaseEntity
{
    public string RxNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? Notes { get; set; }

    // Doctor Info
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorLicenseNumber { get; set; }
    public string? DoctorPhone { get; set; }
    public string? ClinicName { get; set; }

    // Foreign Keys
    public Guid PatientId { get; set; }
    public Guid? VerifiedByUserId { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public User? VerifiedBy { get; set; }
    public ICollection<PrescriptionItem> Items { get; set; } = [];
    public ICollection<Sale> Sales { get; set; } = [];
}