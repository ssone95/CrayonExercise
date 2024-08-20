using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;

namespace Crayon.API.Infrastructure.Services.Interfaces;

public interface IOrderService
{
    Task<OrderServicesResponse> Post(OrderServicesRequest request);
    Task<GetOrdersByAccountResponse> GetByAccount(GetOrdersByAccountRequest request);
    Task<UpdateSubscriptionResponse> UpdateSubscription(UpdateSubscriptionRequest request);
}