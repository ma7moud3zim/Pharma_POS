using Microsoft.AspNetCore.SignalR;
using PharmaPOS.API.Hubs;

namespace PharmaPOS.API.Services;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendLowStockAlertAsync(string drugName, int currentStock)
    {
        await _hubContext.Clients.Group("Managers").SendAsync("LowStockAlert", new
        {
            drugName,
            currentStock,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendExpiryAlertAsync(string drugName, string batchNumber, DateTime expiryDate)
    {
        await _hubContext.Clients.Group("Pharmacists").SendAsync("ExpiryAlert", new
        {
            drugName,
            batchNumber,
            expiryDate,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendNewPrescriptionAlertAsync(string rxNumber, string patientName)
    {
        await _hubContext.Clients.Group("Pharmacists").SendAsync("NewPrescription", new
        {
            rxNumber,
            patientName,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendSaleCompletedAlertAsync(string invoiceNumber, decimal totalAmount)
    {
        await _hubContext.Clients.Group("Admins").SendAsync("SaleCompleted", new
        {
            invoiceNumber,
            totalAmount,
            timestamp = DateTime.UtcNow
        });
    }
}