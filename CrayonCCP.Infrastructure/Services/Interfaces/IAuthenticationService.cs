using CrayonCCP.Domain.DTOs.Requests.Internal;
using CrayonCCP.Domain.DTOs.Responses.Base;
using CrayonCCP.Domain.Enums;
using CrayonCCP.Infrastructure.Services.Models.Responses.Authentication.Parts;

namespace CrayonCCP.Infrastructure.Services.Interfaces;

public interface IAuthenticationService
{
    Task<BaseResponse<AuthenticationResponsePart>> AuthenticateAsync(AuthenticateClientRequest request);
}