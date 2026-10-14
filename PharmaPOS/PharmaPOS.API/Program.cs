using Microsoft.AspNetCore.Authentication.JwtBearer;
using PharmaPOS.API.Services;
using PharmaPOS.Infrastructure;
using PharmaPOS.Infrastructure.Data;
using PharmaPOS.Infrastructure.Seeders;
using Serilog;
using FluentValidation;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Yarp.ReverseProxy;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/pharmapos-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();


// Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpClient();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = false,
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
            if (claimsIdentity == null) return Task.CompletedTask;

            var realmAccess = context.Principal?.FindFirst("realm_access")?.Value;
            if (realmAccess != null)
            {
                var parsed = System.Text.Json.JsonDocument.Parse(realmAccess);
                if (parsed.RootElement.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                            System.Security.Claims.ClaimTypes.Role,
                            role.GetString()!));
                    }
                }
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "PharmaPOS_";
});
builder.Services.AddScoped<CacheService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddAuthorization();



var app = builder.Build();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<PharmaPOSSeeder>();
    await seeder.SeedAsync();
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();