using System.Security.Claims;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crayon.API.Infrastructure.Services.Middlewares;

public class JWTAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = context.RequestServices.CreateScope();
        var customerAccountSvc = scope.ServiceProvider.GetRequiredService<ICustomerDataService>();
        var userAccountSvc = scope.ServiceProvider.GetRequiredService<IUserDataService>();
        
        var identityUser = context.User;
        var userRoles = identityUser.Claims
            .Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)
            .ToList();

        var userRoleNames = userRoles
            .Select(x => x.Value.ToUpper())
            .Distinct()
            .ToList();
        
        var emailClaim = identityUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var userIdClaim = identityUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

        if (!Guid.TryParse(userIdClaim, out Guid userId) || string.IsNullOrEmpty(emailClaim)
            || !userRoles.Any())
        {
            // We should set the authentication context to 
            context.Items.Add("UserContext", new CommonUserContextModel()
            {
                Roles = [],
                UserEmail = string.Empty,
                UserId = Guid.Empty,
                IsAdmin = false,
                IsAuthenticated = false,
                IsCustomer = false
            });
            await next(context);
            return;
        }
        
        List<string> customerRoles = ["CUSTOMER", "SUBCUSTOMER", "MANAGER", "BROKER"];
        CommonUserContextModel userCtx;
        if (userRoles.Any(x => customerRoles.Contains(x.Value.ToUpper())))
        {
            userCtx = await PrepareCustomerUserContextModel(userRoleNames, userId, emailClaim, customerAccountSvc);
        }
        else
        {
            userCtx = PrepareAdminUserContextModel(userRoleNames, emailClaim, 
                userRoles.Any(x => x.Value.ToUpper() == "ADMIN"), 
                userId, userAccountSvc);
        }
        context.Items.Add("UserContext", userCtx);
        await next(context);
    }

    private CommonUserContextModel PrepareAdminUserContextModel(List<string> userRoleNames, string emailClaim, bool isAdmin, Guid userId, 
        IUserDataService userDataService)
    {
        // TODO: Replace this with real retrieval and mapping of user acc entity
        return new CommonUserContextModel()
        {
            Roles = userRoleNames,
            UserEmail = emailClaim,
            UserId = userId,
            IsAdmin = isAdmin,
            IsCustomer = false,
            IsAuthenticated = true
        };
    }

    private async Task<CommonUserContextModel> PrepareCustomerUserContextModel(List<string> userRoleNames, Guid userId,
        string emailClaim, ICustomerDataService customerDataService)
    {
        CommonUserContextModel? result = await customerDataService.GetCustomerEntityAsCtxModelById(userId);

        if (result is not null)
            result.Roles.AddRange(userRoleNames);

        return result ?? new()
        {
            Roles = [],
            IsAuthenticated = false,
            IsCustomer = false,
            IsAdmin = false,
            UserId = userId,
            UserEmail = emailClaim
        };
    }
}