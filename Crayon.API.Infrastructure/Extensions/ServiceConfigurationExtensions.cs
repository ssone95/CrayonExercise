using Crayon.API.Infrastructure.Repositories;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Crayon.API.Infrastructure.Services;
using Crayon.API.Infrastructure.Services.Interfaces;
using Crayon.API.Infrastructure.Services.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper.Configuration;
using Crayon.API.Domain.MappingProfiles;

namespace Crayon.API.Infrastructure.Extensions;

public static class ServiceConfigurationExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerAccountRepository, CustomerAccountRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderLineRepository, OrderLineRepository>();
        services.AddScoped<ILicenseRepository, LicenseRepository>();

        var modelAssembly =
            typeof(CustomerAccountToCustomerRegistrationResponseProfile).Assembly;
        services.AddAutoMapper(o =>
        {
            o.AddMaps(modelAssembly);
        });
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticatorService, AuthenticatorService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<ICustomerDataService, CustomerDataService>();
        services.AddScoped<IUserDataService, UserDataService>();

        services.AddScoped<ICCPDataService, CCPDataService>();
        services.AddScoped<IOrderService, OrderService>();
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<JWTAuthenticationMiddleware>();
        // app.UseMiddleware<ApiKeyAuthMiddleware>();
    }
}