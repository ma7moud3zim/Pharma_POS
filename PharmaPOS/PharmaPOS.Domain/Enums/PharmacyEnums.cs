namespace PharmaPOS.Domain.Enums;

public enum UserRole
{
    Admin = 1,
    Pharmacist = 2,
    Cashier = 3,
    Manager = 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum DrugCategory
{
    OTC = 1,           
    Prescription = 2,
    Controlled = 3, 
    Supplement = 4,
    Cosmetic = 5,
    Equipment = 6
}

public enum DrugForm
{
    Tablet = 1,
    Capsule = 2,
    Syrup = 3,
    Injection = 4,
    Cream = 5,
    Drops = 6,
    Inhaler = 7,
    Suppository = 8,
    Patch = 9,
    Powder = 10
}

public enum SaleStatus
{
    Pending = 1,
    Completed = 2,
    Refunded = 3,
    PartialRefund = 4,
    Cancelled = 5
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    Insurance = 4,
    MobileWallet = 5,
    Mixed = 6
}

public enum PrescriptionStatus
{
    Pending = 1,
    Verified = 2,
    Dispensed = 3,
    Rejected = 4,
    Expired = 5
}

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Received = 4,
    PartiallyReceived = 5,
    Cancelled = 6
}

public enum StockAdjustmentReason
{
    DamagedGoods = 1,
    ExpiredProduct = 2,
    InventoryCorrection = 3,
    ReturnToSupplier = 4,
    TheftOrLoss = 5,
    InitialStock = 6
}

public enum DiscountType
{
    Percentage = 1,
    FixedAmount = 2
}

public enum NotificationType
{
    LowStock = 1,
    ExpiryAlert = 2,
    PrescriptionReady = 3,
    LoyaltyMilestone = 4,
    SystemAlert = 5
}