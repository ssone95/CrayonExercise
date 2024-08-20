using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Domain.DTOs.Responses.External.Orders;

namespace Crayon.API.Infrastructure.Services.Interfaces;

public interface ICCPDataService : IDisposable
{
    Task<ListServicesResponse> GetAvailableServices(ListServicesRequest request);
    Task<OrderServicesResponse> SubmitOrder(OrderServicesRequest request, Guid orderId);
    new void Dispose();
}