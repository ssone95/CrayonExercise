namespace Crayon.API.Domain.Enums;

public enum LicenseStatus
{
    Terminated = -3,
    Cancelled,
    Expired,
    Pending,
    Processing,
    Active,
    
    Lifetime = 99
}