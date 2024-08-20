using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Internal.UserAuthentication;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Domain.DTOs.Responses.Internal.UserAuthentication;

namespace Crayon.API.Infrastructure.Services.Interfaces;

public interface IAuthenticatorService
{
    public Task<LoginResponse> Login(AuthenticateUserBasicRequest request);

    public Task<AuthenticationResponse> AuthenticateJWT(AuthenticateUserJwtRequest request);
}