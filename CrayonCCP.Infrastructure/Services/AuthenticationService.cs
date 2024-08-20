using System.Security.Claims;
using CrayonCCP.Domain.DTOs.Requests.Internal;
using CrayonCCP.Domain.DTOs.Responses.Base;
using CrayonCCP.Domain.Entities.Clients;
using CrayonCCP.Domain.Enums;
using CrayonCCP.Infrastructure.Extensions;
using CrayonCCP.Infrastructure.Repositories.Interfaces;
using CrayonCCP.Infrastructure.Services.Interfaces;
using CrayonCCP.Infrastructure.Services.Models.Responses.Authentication.Parts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace CrayonCCP.Infrastructure.Services;

public class AuthenticationService(
    IApiKeyRepository apiKeyRepository,
    IClientRepository clientRepository,
    ILogger<AuthenticationService> logger)
    : IAuthenticationService
{
    private readonly IApiKeyRepository _apiKeyRepository = apiKeyRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<BaseResponse<AuthenticationResponsePart>> AuthenticateAsync(AuthenticateClientRequest request)
    {
        byte[] key = Sha256Extensions.Hash(request.ApiKey);
        var apiKey = await _apiKeyRepository.FindByHashValueAsync(key);

        if (apiKey is null || apiKey.ClientId is null)
        {
            return BuildFailedResponse(AuthenticationResultEnum.Unauthorized, "Unauthorized!");
        }

        if (apiKey.ExpiresOn <= DateTime.UtcNow)
        {
            return BuildFailedResponse(AuthenticationResultEnum.Forbidden, "Forbidden!");
        }

        var client = await _clientRepository.GetByIdAsync(apiKey.ClientId.Value);
        if (client is null)
        {
            return BuildFailedResponse(AuthenticationResultEnum.Unauthorized, "Unauthorized");
        }

        return BuildSuccessfulResponse(apiKey, client);
    }

    private BaseResponse<AuthenticationResponsePart> BuildSuccessfulResponse(ApiKeyEntity apiKey, ClientEntity client)
    {
        ClaimsIdentity claimsIdentity = new() { Claims = {} };
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, client.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Spn, client.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, client.Identifier.ToString()));
        
        return new BaseResponse<AuthenticationResponsePart>()
        {
            Data = new AuthenticationResponsePart()
            {
                Result = AuthenticationResultEnum.Success,
                Client = claimsIdentity
            },
            Success = true
        };
    }

    private static BaseResponse<AuthenticationResponsePart> BuildFailedResponse(AuthenticationResultEnum authenticationResult,
        string? message)
    {
        return new()
        {
            Data = new()
            {
                Result = authenticationResult,
                Client = null
            },
            Success = authenticationResult != AuthenticationResultEnum.Success,
            Message = message
        };
    }
}