namespace CrayonCCP.Domain.DTOs.Requests.Internal;

public class AuthenticateClientRequest
{
    public required string ApiKey { get; init; }
}