using Crayon.API.Domain.DTOs.Requests.Common;
using Microsoft.AspNetCore.Http;

namespace Crayon.API.Infrastructure.Extensions;

public static class UserContextExtensions
{
    public static T GetUserCtx<T>(this HttpContext context) where T : CommonUserContextModel
    {
        if (context.Items.TryGetValue("UserContext", out var ctx) && ctx is T userContext)
        {
            return userContext;
        }

        throw new ArgumentNullException("UserContext",
            "UserContext couldn't be retrieved, authentication is having some issues!");
    }
}