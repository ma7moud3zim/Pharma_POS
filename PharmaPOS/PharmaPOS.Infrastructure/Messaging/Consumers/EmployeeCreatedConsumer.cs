using MassTransit;
using Microsoft.Extensions.Logging;
using PharmaPOS.Contracts.Events;
using PharmaPOS.Domain.Entities;
using PharmaPOS.Domain.Enums;
using PharmaPOS.Infrastructure.Data;

namespace PharmaPOS.Infrastructure.Messaging.Consumers;

public class EmployeeCreatedConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly PharmaPOSDbContext _context;
    private readonly ILogger<EmployeeCreatedConsumer> _logger;

    public EmployeeCreatedConsumer(PharmaPOSDbContext context, ILogger<EmployeeCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        _logger.LogInformation("Received EmployeeCreatedEvent for {Email}", context.Message.Email);

        if (!Enum.TryParse<UserRole>(context.Message.Role, true, out var role))
            role = UserRole.Cashier;

        var user = new User
        {
            FullName = context.Message.FullName,
            Email = context.Message.Email,
            PhoneNumber = context.Message.PhoneNumber,
            Role = role,
            PasswordHash = string.Empty,
            IsActive = true
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created in PharmaPOS for employee {Email}", context.Message.Email);
    }
}

public class EmployeeDeletedConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly PharmaPOSDbContext _context;
    private readonly ILogger<EmployeeDeletedConsumer> _logger;

    public EmployeeDeletedConsumer(PharmaPOSDbContext context, ILogger<EmployeeDeletedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        _logger.LogInformation("Received EmployeeDeletedEvent for {Email}", context.Message.Email);

        var user = _context.Users.FirstOrDefault(u => u.Email == context.Message.Email);
        if (user is null) return;

        user.IsActive = false;
        user.IsDeleted = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deactivated in PharmaPOS for employee {Email}", context.Message.Email);
    }
}