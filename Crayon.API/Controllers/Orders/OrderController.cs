using System.Security.Authentication;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;
using Crayon.API.Infrastructure.Extensions;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Crayon.API.Controllers.Orders;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService orderService) : Controller
{
    [Authorize(Roles = "ADMIN,MANAGER")]
    [HttpPost("[action]")]
    [Consumes(typeof(OrderServicesRequest), "application/json")]
    [ProducesResponseType(typeof(OrderServicesResponse), 200, "application/json")]
    [ProducesResponseType(typeof(OrderServicesResponse), 400, "application/json")]
    [ProducesResponseType(typeof(object), 400, "application/json")]
    public async Task<IActionResult> Post([FromBody] OrderServicesRequest request)
    {
        var userContext = HttpContext.GetUserCtx<CustomerAccountDetailsContextModel>();
        if (!userContext.IsAuthenticated) throw new AuthenticationException("Temporary error, please check your credentials and re-login!");

        request.UserContext = userContext;
        
        OrderServicesResponse response = await orderService.Post(request);
        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }
    
    [Authorize(Roles = "ADMIN,MANAGER,SUBCUSTOMER")]
    [HttpGet("byAccount/{accountId}/list")]
    [ProducesResponseType(typeof(GetOrdersByAccountResponse), 200, "application/json")]
    [ProducesResponseType(typeof(GetOrdersByAccountResponse), 400, "application/json")]
    [ProducesResponseType(typeof(object), 400, "application/json")]
    public async Task<IActionResult> List(Guid accountId)
    {
        var userContext = HttpContext.GetUserCtx<CustomerAccountDetailsContextModel>();
        if (!userContext.IsAuthenticated) throw new AuthenticationException("Temporary error, please check your credentials and re-login!");

        GetOrdersByAccountRequest request = new()
        {
            AccountId = accountId,
            UserContext = userContext
        };
        
        GetOrdersByAccountResponse response = await orderService.GetByAccount(request);
        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }
    
    [Authorize(Roles = "ADMIN,MANAGER")]
    [HttpPost("{orderId}/updateSubscription/{serviceId}")]
    [Consumes(typeof(UpdateSubscriptionRequest), "application/json")]
    [ProducesResponseType(typeof(GetOrdersByAccountResponse), 200, "application/json")]
    [ProducesResponseType(typeof(GetOrdersByAccountResponse), 400, "application/json")]
    [ProducesResponseType(typeof(object), 400, "application/json")]
    public async Task<IActionResult> UpdateSubscription(Guid orderId, Guid serviceId, [FromBody] UpdateSubscriptionRequest request)
    {
        var userContext = HttpContext.GetUserCtx<CustomerAccountDetailsContextModel>();
        if (!userContext.IsAuthenticated) throw new AuthenticationException("Temporary error, please check your credentials and re-login!");

        request.UserContext = userContext;
        request.OrderId = orderId;
        request.ServiceId = serviceId;

        UpdateSubscriptionResponse response = await orderService.UpdateSubscription(request);

        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }
}