using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Internal.UserAuthentication;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Domain.DTOs.Responses.External.Parts;
using Crayon.API.Domain.DTOs.Responses.Internal.UserAuthentication;
using Crayon.API.Infrastructure.Extensions;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Crayon.API.Infrastructure.Services.Interfaces;
using CrayonCCP.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RTools_NTS.Util;

namespace Crayon.API.Infrastructure.Services;

public class AuthenticatorService(IUserAuthenticationRepository authenticationRepository, IConfiguration configuration, ILogger<AuthenticatorService> logger) : IAuthenticatorService
{
    public async Task<LoginResponse> Login(AuthenticateUserBasicRequest request)
    {
        if (!request.IsCustomer)
        {
            return await AuthenticateUserAndFormatResponse(request);
        }

        return await AuthenticateCustomerAndFormatResponse(request);
    }

    private async Task<LoginResponse> AuthenticateCustomerAndFormatResponse(AuthenticateUserBasicRequest request)
    {
        string jwtSecret = configuration.GetSection("Security:Jwt").GetValue<string>("Secret")!;
        string jwtIssuer = configuration.GetSection("Security:Jwt").GetValue<string>("Issuer")!;
        string jwtAudience = configuration.GetSection("Security:Jwt").GetValue<string>("Audience")!;
        int jwtTokenExpirationMinutes = configuration.GetSection("Security:Jwt").GetValue<int>("TokenDurationMinutes");
        
        byte[] bytes = Sha256Extensions.Hash(request.LoginRequest.Password);
        var customerEntity =
            await authenticationRepository.AuthenticateCustomerBasic(request.LoginRequest.Username, bytes);
        
        if (customerEntity is null)
        {
            logger.LogWarning("Authentication failed due to invalid credentials for user {user}!", request.LoginRequest.Username);
            return new LoginResponse()
            {
                Success = false,
                Message = "Invalid credentials!",
                Data = null
            };
        }

        (string token, DateTime expirationDate) =
            JwtHelpers.GenerateToken(customerEntity, TimeSpan.FromMinutes(jwtTokenExpirationMinutes), jwtSecret, jwtIssuer, jwtAudience);
        
        return new()
        {
            Success = true,
            Data = new()
            {
                AuthToken = token,
                ExpirationDate = expirationDate
            }
        };
    }

    private async Task<LoginResponse> AuthenticateUserAndFormatResponse(AuthenticateUserBasicRequest request)
    {
        string jwtSecret = configuration.GetSection("Security:Jwt").GetValue<string>("Secret")!;
        string jwtIssuer = configuration.GetSection("Security:Jwt").GetValue<string>("Issuer")!;
        string jwtAudience = configuration.GetSection("Security:Jwt").GetValue<string>("Audience")!;
        int jwtTokenExpirationMinutes = configuration.GetSection("Security:Jwt").GetValue<int>("TokenDurationMinutes");
        byte[] bytes = Sha256Extensions.Hash(request.LoginRequest.Password);
        var result = string.Join("", bytes.Select(x => x.ToString("X2")));
        var userEntity = await authenticationRepository.AuthenticateBasic(request.LoginRequest.Username, bytes);
        if (userEntity is null)
        {
            logger.LogWarning("Authentication failed due to invalid credentials for user {user}!", request.LoginRequest.Username);
            return new LoginResponse()
            {
                Success = false,
                Message = "Invalid credentials!",
                Data = null
            };
        }

        (string token, DateTime expirationDate) =
            JwtHelpers.GenerateToken(userEntity, TimeSpan.FromMinutes(jwtTokenExpirationMinutes), jwtSecret, jwtIssuer, jwtAudience);
        
        return new()
        {
            Success = true,
            Data = new()
            {
                AuthToken = token,
                ExpirationDate = expirationDate
            }
        };
    }

    public async Task<AuthenticationResponse> AuthenticateJWT(AuthenticateUserJwtRequest request)
    {
        await Task.Delay(300);
        throw new NotImplementedException();
    }
}