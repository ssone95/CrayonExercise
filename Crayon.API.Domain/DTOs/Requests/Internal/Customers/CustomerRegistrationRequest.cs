using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.External;

namespace Crayon.API.Domain.DTOs.Requests.Internal.Customers;

public class CustomerRegistrationRequest
{
    public required RegisterRequest RegistrationRequest { get; init; }
    public bool SubAccountRegistration { get; init; }
    public bool IsServiceBroker { get; init; }
    public CustomerAccountDetailsContextModel? UserContext { get; init; }
}