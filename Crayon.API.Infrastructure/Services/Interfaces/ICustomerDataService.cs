using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;

namespace Crayon.API.Infrastructure.Services.Interfaces;

public interface ICustomerDataService
{
    Task<CustomerAccountListResponse> GetCustomerAccounts(CustomerAccountListRequest request);
    Task<CommonUserContextModel?> GetCustomerEntityAsCtxModelById(Guid id);
}