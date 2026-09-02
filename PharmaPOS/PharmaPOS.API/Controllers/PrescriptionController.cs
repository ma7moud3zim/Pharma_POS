using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Prescription;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public PrescriptionsController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prescriptions = await _context.Prescriptions
            .Include(rx => rx.Patient)
            .Include(rx => rx.VerifiedBy)
            .Include(rx => rx.Items)
                .ThenInclude(i => i.Drug)
            .Select(rx => new PrescriptionDto
            {
                Id = rx.Id,
                RxNumber = rx.RxNumber,
                PatientName = rx.Patient.FullName,
                DoctorName = rx.DoctorName,
                DoctorLicenseNumber = rx.DoctorLicenseNumber,
                DoctorPhone = rx.DoctorPhone,
                ClinicName = rx.ClinicName,
                IssuedDate = rx.IssuedDate,
                ExpiryDate = rx.ExpiryDate,
                Status = rx.Status.ToString(),
                RejectionReason = rx.RejectionReason,
                VerifiedBy = rx.VerifiedBy != null ? rx.VerifiedBy.FullName : null,
                VerifiedAt = rx.VerifiedAt,
                ImageUrl = rx.ImageUrl,
                Notes = rx.Notes,
                Items = rx.Items.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    DrugNameAsWritten = i.DrugNameAsWritten,
                    PrescribedQuantity = i.PrescribedQuantity,
                    DispensedQuantity = i.DispensedQuantity,
                    Dosage = i.Dosage,
                    Duration = i.Duration,
                    Instructions = i.Instructions,
                    IsSubstitutionAllowed = i.IsSubstitutionAllowed
                }).ToList(),
                CreatedAt = rx.CreatedAt
            })
            .ToListAsync();

        return Ok(prescriptions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var prescription = await _context.Prescriptions
            .Include(rx => rx.Patient)
            .Include(rx => rx.VerifiedBy)
            .Include(rx => rx.Items)
                .ThenInclude(i => i.Drug)
            .Where(rx => rx.Id == id)
            .Select(rx => new PrescriptionDto
            {
                Id = rx.Id,
                RxNumber = rx.RxNumber,
                PatientName = rx.Patient.FullName,
                DoctorName = rx.DoctorName,
                DoctorLicenseNumber = rx.DoctorLicenseNumber,
                DoctorPhone = rx.DoctorPhone,
                ClinicName = rx.ClinicName,
                IssuedDate = rx.IssuedDate,
                ExpiryDate = rx.ExpiryDate,
                Status = rx.Status.ToString(),
                RejectionReason = rx.RejectionReason,
                VerifiedBy = rx.VerifiedBy != null ? rx.VerifiedBy.FullName : null,
                VerifiedAt = rx.VerifiedAt,
                ImageUrl = rx.ImageUrl,
                Notes = rx.Notes,
                Items = rx.Items.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    DrugNameAsWritten = i.DrugNameAsWritten,
                    PrescribedQuantity = i.PrescribedQuantity,
                    DispensedQuantity = i.DispensedQuantity,
                    Dosage = i.Dosage,
                    Duration = i.Duration,
                    Instructions = i.Instructions,
                    IsSubstitutionAllowed = i.IsSubstitutionAllowed
                }).ToList(),
                CreatedAt = rx.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (prescription is null)
            return NotFound(new { message = $"Prescription with ID {id} not found." });

        return Ok(prescription);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        var prescriptions = await _context.Prescriptions
            .Include(rx => rx.Patient)
            .Include(rx => rx.VerifiedBy)
            .Include(rx => rx.Items)
                .ThenInclude(i => i.Drug)
            .Where(rx => rx.PatientId == patientId)
            .Select(rx => new PrescriptionDto
            {
                Id = rx.Id,
                RxNumber = rx.RxNumber,
                PatientName = rx.Patient.FullName,
                DoctorName = rx.DoctorName,
                DoctorLicenseNumber = rx.DoctorLicenseNumber,
                DoctorPhone = rx.DoctorPhone,
                ClinicName = rx.ClinicName,
                IssuedDate = rx.IssuedDate,
                ExpiryDate = rx.ExpiryDate,
                Status = rx.Status.ToString(),
                RejectionReason = rx.RejectionReason,
                VerifiedBy = rx.VerifiedBy != null ? rx.VerifiedBy.FullName : null,
                VerifiedAt = rx.VerifiedAt,
                ImageUrl = rx.ImageUrl,
                Notes = rx.Notes,
                Items = rx.Items.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    DrugName = i.Drug.Name,
                    DrugNameAsWritten = i.DrugNameAsWritten,
                    PrescribedQuantity = i.PrescribedQuantity,
                    DispensedQuantity = i.DispensedQuantity,
                    Dosage = i.Dosage,
                    Duration = i.Duration,
                    Instructions = i.Instructions,
                    IsSubstitutionAllowed = i.IsSubstitutionAllowed
                }).ToList(),
                CreatedAt = rx.CreatedAt
            })
            .ToListAsync();

        return Ok(prescriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto)
    {
        var patient = await _context.Patients.FindAsync(dto.PatientId);
        if (patient is null)
            return NotFound(new { message = "Patient not found." });

        var rxCount = await _context.Prescriptions.CountAsync();
        var rxNumber = $"RX-{DateTime.UtcNow.Year}-{(rxCount + 1):D5}";

        var prescription = new Prescription
        {
            RxNumber = rxNumber,
            PatientId = dto.PatientId,
            DoctorName = dto.DoctorName,
            DoctorLicenseNumber = dto.DoctorLicenseNumber,
            DoctorPhone = dto.DoctorPhone,
            ClinicName = dto.ClinicName,
            IssuedDate = dto.IssuedDate,
            ExpiryDate = dto.ExpiryDate,
            Notes = dto.Notes,
            Status = PrescriptionStatus.Pending,
            Items = dto.Items.Select(i => new PrescriptionItem
            {
                DrugId = i.DrugId,
                DrugNameAsWritten = i.DrugNameAsWritten,
                PrescribedQuantity = i.PrescribedQuantity,
                Dosage = i.Dosage,
                Duration = i.Duration,
                Instructions = i.Instructions,
                IsSubstitutionAllowed = i.IsSubstitutionAllowed
            }).ToList()
        };

        await _context.Prescriptions.AddAsync(prescription);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, new { id = prescription.Id });
    }

    [HttpPatch("{id:guid}/verify")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyPrescriptionDto dto)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription is null)
            return NotFound(new { message = $"Prescription with ID {id} not found." });

        if (prescription.Status != PrescriptionStatus.Pending)
            return BadRequest(new { message = "Only pending prescriptions can be verified." });

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);

        prescription.Status = dto.IsApproved
            ? PrescriptionStatus.Verified
            : PrescriptionStatus.Rejected;

        prescription.RejectionReason = dto.IsApproved ? null : dto.RejectionReason;
        prescription.VerifiedByUserId = user?.Id;
        prescription.VerifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription is null)
            return NotFound(new { message = $"Prescription with ID {id} not found." });

        if (prescription.Status == PrescriptionStatus.Dispensed)
            return BadRequest(new { message = "Cannot delete a dispensed prescription." });

        prescription.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}