namespace Crayon.API.Domain.Enums;

public enum EntityStatus
{
    Archived = -1,
    Inactive,
    Active,
    
    Expired = 100,
    Cancelled = 101
}