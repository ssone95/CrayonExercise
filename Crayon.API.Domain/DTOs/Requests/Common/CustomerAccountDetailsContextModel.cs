namespace Crayon.API.Domain.DTOs.Requests.Common;

public class CustomerAccountDetailsContextModel : CommonUserContextModel
{
    public required Guid CustomerAccountId { get; init; }
    public int InternalCustomerAccountId { get; init; }
    public required Guid CustomerId { get; init; }
    public int InternalCustomerId { get; init; }
    
    public bool IsManager { get; init; }
    public bool IsBroker { get; init; }
}