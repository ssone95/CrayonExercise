using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using CrayonCCP.Domain.Enums;
using CrayonCCP.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrayonCCP.Infrastructure.Services.Middlewares;

public class ApiKeyAuthMiddleware
{
    private const string ApiKeyHeader = "X-API-KEY";
    private readonly IServiceProvider _provider;
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    
    public ApiKeyAuthMiddleware(
        RequestDelegate next,
        IServiceProvider provider,
        ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _provider = provider;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        string traceId = context.Request.Headers["X-Trace-Id"]!;
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKeyHeader)
            || string.IsNullOrEmpty(apiKeyHeader))
        {
            _logger.LogWarning("Api key for request {traceId} wasn't provided!", traceId);
            throw new AuthenticationException("API Key must be provided!");
        }

        using var scope = _provider.CreateScope();
        var authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        var authResult = await authenticationService.AuthenticateAsync(new()
        {
            ApiKey = apiKeyHeader!
        });

        if (!authResult.Success || authResult.Data!.Client is null)
        {
            _logger.LogWarning("An error occurred while trying to authenticate the request {traceId}: {message}!", traceId, authResult.Message);
            if (authResult.Data!.Result == AuthenticationResultEnum.Forbidden) throw new UnauthorizedAccessException(authResult.Message);
            else throw new AuthenticationException(authResult.Message);
        }

        context.User = new ClaimsPrincipal()
        {
            Identities = {}
        };
        context.User.AddIdentity(authResult.Data.Client);
        
        await _next(context);
    }
}