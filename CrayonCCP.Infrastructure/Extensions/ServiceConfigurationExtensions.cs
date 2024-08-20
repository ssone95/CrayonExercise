using CrayonCCP.Infrastructure.Repositories;
using CrayonCCP.Infrastructure.Repositories.Interfaces;
using CrayonCCP.Infrastructure.Services;
using CrayonCCP.Infrastructure.Services.Interfaces;
using CrayonCCP.Infrastructure.Services.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRedisStack.Extensions;
using StackExchange.Redis;

namespace CrayonCCP.Infrastructure.Extensions;

public static class ServiceConfigurationExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<ApiKeyAuthMiddleware>();
    }
}