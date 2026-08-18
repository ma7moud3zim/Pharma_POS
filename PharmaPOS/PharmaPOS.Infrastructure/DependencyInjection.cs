using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmaPOS.Infrastructure.Data;
using PharmaPOS.Infrastructure.Repositories.Implementations;
using PharmaPOS.Infrastructure.Repositories.Interfaces;
using PharmaPOS.Infrastructure.Seeders;

namespace PharmaPOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PharmaPOSDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(PharmaPOSDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Seeder
        services.AddScoped<PharmaPOSSeeder>();

        return services;
    }
}