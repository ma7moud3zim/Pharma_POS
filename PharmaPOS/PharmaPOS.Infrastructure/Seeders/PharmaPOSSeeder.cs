using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.Infrastructure.Seeders;

public class PharmaPOSSeeder(PharmaPOSDbContext db, ILogger<PharmaPOSSeeder> logger)
{
    public async Task SeedAsync()
    {
        try
        {
            await db.Database.MigrateAsync();
            await SeedUsersAsync();
            await SeedSuppliersAsync();
            await SeedDrugsAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await db.Users.AnyAsync()) return;

        var users = new List<User>
        {
            new()
            {
                FullName = "System Administrator",
                Email = "admin@pharmapos.com",
                PasswordHash = "$2a$12$kVf4Ec8Tl3/oq4j.DkQAe.9e4JZrGfBbzL3f6uIbOfJ6FIPoBJ/u",
                Role = UserRole.Admin,
                IsActive = true
            },
            new()
            {
                FullName = "Head Pharmacist",
                Email = "pharmacist@pharmapos.com",
                PasswordHash = "$2a$12$kVf4Ec8Tl3/oq4j.DkQAe.9e4JZrGfBbzL3f6uIbOfJ6FIPoBJ/u",
                Role = UserRole.Pharmacist,
                IsActive = true
            },
            new()
            {
                FullName = "Cashier One",
                Email = "cashier@pharmapos.com",
                PasswordHash = "$2a$12$kVf4Ec8Tl3/oq4j.DkQAe.9e4JZrGfBbzL3f6uIbOfJ6FIPoBJ/u",
                Role = UserRole.Cashier,
                IsActive = true
            }
        };

        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} users.", users.Count);
    }

    private async Task SeedSuppliersAsync()
    {
        if (await db.Suppliers.AnyAsync()) return;

        var suppliers = new List<Supplier>
        {
            new()
            {
                Name = "Pharma Distributors",
                ContactPerson = "Ahmed Hassan",
                PhoneNumber = "+20-100-111-2222",
                Email = "sales@pharmadist.com",
                PaymentTermDays = 30,
                IsActive = true
            }
        };

        await db.Suppliers.AddRangeAsync(suppliers);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} suppliers.", suppliers.Count);
    }

    private async Task SeedDrugsAsync()
    {
        if (await db.Drugs.AnyAsync()) return;

        var drugs = new List<Drug>
        {
            new()
            {
                Name = "Paracetamol 500mg",
                GenericName = "Acetaminophen",
                Barcode = "6221234567890",
                SKU = "PARA-500",
                Category = DrugCategory.OTC,
                Form = DrugForm.Tablet,
                Strength = "500mg",
                Manufacturer = "EIPICO",
                CostPrice = 5.00m,
                SellingPrice = 8.50m,
                RequiresPrescription = false,
                ReorderLevel = 50,
                ReorderQuantity = 200
            },
            new()
            {
                Name = "Amoxicillin 500mg",
                GenericName = "Amoxicillin",
                Barcode = "6221234567891",
                SKU = "AMOX-500",
                Category = DrugCategory.Prescription,
                Form = DrugForm.Capsule,
                Strength = "500mg",
                Manufacturer = "GlaxoSmithKline",
                CostPrice = 25.00m,
                SellingPrice = 38.00m,
                RequiresPrescription = true,
                ReorderLevel = 20,
                ReorderQuantity = 100
            }
        };

        await db.Drugs.AddRangeAsync(drugs);
        await db.SaveChangesAsync();

        // loggin with serilog to make a searchable poperty
        logger.LogInformation("Seeded {Count} drugs.", drugs.Count);
    }
}