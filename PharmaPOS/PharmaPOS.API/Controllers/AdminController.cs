using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly PharmaPOSDbContext _context;

    public AdminController(PharmaPOSDbContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                Role = u.Role.ToString(),
                u.IsActive,
                u.LastLoginAt,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetSales()
    {
        var sales = await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.Items)
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new
            {
                s.Id,
                s.InvoiceNumber,
                s.SaleDate,
                Cashier = s.Cashier.FullName,
                s.TotalAmount,
                s.SubTotal,
                s.DiscountAmount,
                s.TaxAmount,
                s.AmountPaid,
                PaymentMethod = s.PaymentMethod.ToString(),
                Status = s.Status.ToString(),
                ItemCount = s.Items.Count
            })
            .ToListAsync();

        return Ok(sales);
    }
}