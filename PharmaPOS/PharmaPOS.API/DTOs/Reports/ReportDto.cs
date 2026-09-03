namespace PharmaPOS.API.DTOs.Reports;

public class DailySalesReportDto
{
    public DateTime Date { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal NetRevenue { get; set; }
    public string TopSellingDrug { get; set; } = string.Empty;
}

public class TopSellingDrugDto
{
    public string DrugName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class LowStockReportDto
{
    public string DrugName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public string? SupplierName { get; set; }
}

public class ExpiryReportDto
{
    public string DrugName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
}

public class PatientSalesReportDto
{
    public string PatientName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int TotalVisits { get; set; }
    public decimal TotalSpent { get; set; }
    public int LoyaltyPoints { get; set; }
    public DateTime? LastVisit { get; set; }
}