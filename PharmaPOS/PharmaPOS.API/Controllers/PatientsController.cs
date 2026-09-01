using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.Patient;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public PatientsController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _context.Patients
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender.ToString(),
                Address = p.Address,
                NationalId = p.NationalId,
                InsuranceNumber = p.InsuranceNumber,
                InsuranceProvider = p.InsuranceProvider,
                KnownAllergies = p.KnownAllergies,
                ChronicConditions = p.ChronicConditions,
                LoyaltyPoints = p.LoyaltyPoints,
                TotalSpent = p.TotalSpent,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _context.Patients
            .Where(p => p.Id == id)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender.ToString(),
                Address = p.Address,
                NationalId = p.NationalId,
                InsuranceNumber = p.InsuranceNumber,
                InsuranceProvider = p.InsuranceProvider,
                KnownAllergies = p.KnownAllergies,
                ChronicConditions = p.ChronicConditions,
                LoyaltyPoints = p.LoyaltyPoints,
                TotalSpent = p.TotalSpent,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (patient is null)
            return NotFound(new { message = $"Patient with ID {id} not found." });

        return Ok(patient);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var patients = await _context.Patients
            .Where(p => p.FullName.Contains(term) ||
                        p.PhoneNumber!.Contains(term) ||
                        p.NationalId!.Contains(term))
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender.ToString(),
                Address = p.Address,
                NationalId = p.NationalId,
                InsuranceNumber = p.InsuranceNumber,
                InsuranceProvider = p.InsuranceProvider,
                KnownAllergies = p.KnownAllergies,
                ChronicConditions = p.ChronicConditions,
                LoyaltyPoints = p.LoyaltyPoints,
                TotalSpent = p.TotalSpent,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        if (!string.IsNullOrEmpty(dto.NationalId) &&
            await _context.Patients.AnyAsync(p => p.NationalId == dto.NationalId))
            return Conflict(new { message = "A patient with this National ID already exists." });

        Gender? gender = null;
        if (!string.IsNullOrEmpty(dto.Gender) && Enum.TryParse<Gender>(dto.Gender, true, out var parsedGender))
            gender = parsedGender;

        var patient = new Patient
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Gender = gender,
            Address = dto.Address,
            NationalId = dto.NationalId,
            InsuranceNumber = dto.InsuranceNumber,
            InsuranceProvider = dto.InsuranceProvider,
            KnownAllergies = dto.KnownAllergies,
            ChronicConditions = dto.ChronicConditions
        };

        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, new { id = patient.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient is null)
            return NotFound(new { message = $"Patient with ID {id} not found." });

        if (!string.IsNullOrEmpty(dto.NationalId) &&
            dto.NationalId != patient.NationalId &&
            await _context.Patients.AnyAsync(p => p.NationalId == dto.NationalId))
            return Conflict(new { message = "A patient with this National ID already exists." });

        Gender? gender = null;
        if (!string.IsNullOrEmpty(dto.Gender) && Enum.TryParse<Gender>(dto.Gender, true, out var parsedGender))
            gender = parsedGender;

        patient.FullName = dto.FullName;
        patient.PhoneNumber = dto.PhoneNumber;
        patient.Email = dto.Email;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = gender;
        patient.Address = dto.Address;
        patient.NationalId = dto.NationalId;
        patient.InsuranceNumber = dto.InsuranceNumber;
        patient.InsuranceProvider = dto.InsuranceProvider;
        patient.KnownAllergies = dto.KnownAllergies;
        patient.ChronicConditions = dto.ChronicConditions;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient is null)
            return NotFound(new { message = $"Patient with ID {id} not found." });

        patient.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}