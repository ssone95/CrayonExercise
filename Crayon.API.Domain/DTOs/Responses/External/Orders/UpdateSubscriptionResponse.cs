using Crayon.API.Domain.DTOs.Responses.Base;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;

namespace Crayon.API.Domain.DTOs.Responses.External.Orders;

public class UpdateSubscriptionResponse : BaseResponse<OrderEntryLineModel>
{
    public Guid OrderId { get; init; }
    public Guid ServiceId { get; init; }
}