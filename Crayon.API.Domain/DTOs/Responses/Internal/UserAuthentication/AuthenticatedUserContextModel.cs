namespace Crayon.API.Domain.DTOs.Responses.Internal.UserAuthentication;

public class AuthenticatedUserContextModel
{
    public int Id { get; init; }
    public Guid Identifier { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public bool IsCustomer { get; init; }
    public bool IsManagementAccount { get; init; }
}