using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.API.DTOs.User;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public UsersController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "A user with this email already exists." });

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            return BadRequest(new { message = "Invalid role." });

        var user = new Domain.Entities.User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = role,
            PasswordHash = string.Empty, // Managed by Keycloak
            IsActive = true
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { id = user.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            return BadRequest(new { message = "Invalid role." });

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Role = role;
        user.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        user.IsDeleted = true;
        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}