using Crayon.API.Domain.DTOs.Requests.External;

namespace Crayon.API.Domain.DTOs.Requests.Internal.UserAuthentication;

public class AuthenticateUserBasicRequest
{
    public required LoginRequest LoginRequest { get; set; }
    public bool IsCustomer { get; set; }
}