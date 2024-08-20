namespace Crayon.API.Domain.DTOs.Responses.External.Parts;

public class TokenResponsePart
{
    public required string AuthToken { get; init; }
    public DateTime ExpirationDate { get; init; }
}