using PharmaPOS.Infrastructure.Data;
using System.Security.Claims;

namespace PharmaPOS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?
        .User.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName => _httpContextAccessor.HttpContext?
        .User.FindFirstValue(ClaimTypes.Name);

    public string? IpAddress => _httpContextAccessor.HttpContext?
        .Connection.RemoteIpAddress?.ToString();
}