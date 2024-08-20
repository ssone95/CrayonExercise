using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;

namespace Crayon.API.Infrastructure.Services.Interfaces;

public interface IRegistrationService
{
    Task<CustomerRegistrationResponse> Register(CustomerRegistrationRequest request);
}