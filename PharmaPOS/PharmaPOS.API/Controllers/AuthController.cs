using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PharmaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var keycloakUrl = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "pharmapos-api",
            ["client_secret"] = _configuration["Keycloak:ClientSecret"]!,
            ["username"] = request.Email,
            ["password"] = request.Password
        };

        var response = await _httpClient.PostAsync(keycloakUrl,
            new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
            return Unauthorized(new { message = "Invalid credentials" });

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);

        return Ok(new
        {
            accessToken = tokenResponse.GetProperty("access_token").GetString(),
            refreshToken = tokenResponse.GetProperty("refresh_token").GetString(),
            expiresIn = tokenResponse.GetProperty("expires_in").GetInt32()
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var keycloakUrl = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = "pharmapos-api",
            ["client_secret"] = _configuration["Keycloak:ClientSecret"]!,
            ["refresh_token"] = request.RefreshToken
        };

        var response = await _httpClient.PostAsync(keycloakUrl,
            new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
            return Unauthorized(new { message = "Invalid credentials" });

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);

        return Ok(new
        {
            accessToken = tokenResponse.GetProperty("access_token").GetString(),
            refreshToken = tokenResponse.GetProperty("refresh_token").GetString(),
            expiresIn = tokenResponse.GetProperty("expires_in").GetInt32()
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var keycloakUrl = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/logout";

        var formData = new Dictionary<string, string>
        {
            ["client_id"] = "pharmapos-api",
            ["client_secret"] = _configuration["Keycloak:ClientSecret"]!,
            ["refresh_token"] = request.RefreshToken
        };

        await _httpClient.PostAsync(keycloakUrl, new FormUrlEncodedContent(formData));

        return NoContent();
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);