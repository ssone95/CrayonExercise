namespace Crayon.API.Domain.DTOs.Responses.Internal.Customers.Parts;

public class CustomerAccountEntryModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}