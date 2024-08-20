using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Crayon.API.Infrastructure.Extensions;

public static class AuthenticationConfigurationExtensions
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            if (environment.IsDevelopment())
            {
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        var err = c.Exception;
                        Console.WriteLine($"OnAuthenticationFailed: {err}\nFull data: {c}\n");
                        return Task.CompletedTask;
                    }
                };
            }
            var jwtSection = configuration.GetSection("Security:Jwt");
            var keyStr = jwtSection.GetValue<string>("Secret");
            if (string.IsNullOrEmpty(keyStr)) return;

            var validIssuer = jwtSection.GetValue<string>("Issuer");
            var validAudience = jwtSection.GetValue<string>("Audience");
            
            var key = Encoding.UTF8.GetBytes(keyStr);

            o.RequireHttpsMetadata = !environment.IsDevelopment();
            o.SaveToken = false;
        
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
    }
}