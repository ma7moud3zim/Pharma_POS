using PharmaPOS.Domain.Common;

namespace PharmaPOS.Domain.Entities;

public class PrescriptionItem : BaseEntity
{
    public int PrescribedQuantity { get; set; }
    public int? DispensedQuantity { get; set; }
    public string? DrugNameAsWritten { get; set; }
    public string? Dosage { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public bool IsSubstitutionAllowed { get; set; } = false;

    // Foreign Keys
    public Guid PrescriptionId { get; set; }
    public Guid DrugId { get; set; }

    // Navigation Properties
    public Prescription Prescription { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}