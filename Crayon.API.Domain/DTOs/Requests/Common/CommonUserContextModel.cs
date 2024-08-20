namespace Crayon.API.Domain.DTOs.Requests.Common;

public class CommonUserContextModel
{
    public Guid UserId { get; init; }
    public required string UserEmail { get; init; }
    
    public bool IsAdmin { get; init; }
    
    public bool IsCustomer { get; init; }
    
    public required List<string> Roles { get; init; }
    
    public bool IsAuthenticated { get; init; }
}