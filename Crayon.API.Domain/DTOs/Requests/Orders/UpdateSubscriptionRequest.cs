using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Orders.Parts;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Domain.DTOs.Requests.Orders;

public class UpdateSubscriptionRequest
{
    [JsonIgnore]
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Guid ServiceId { get; set; }
    
    public SubscriptionRequestOperation RequestType { get; init; }
    
    [JsonIgnore]
    public CustomerAccountDetailsContextModel? UserContext { get; set; }
    
    public UpdateQuantityPart? QuantityPart { get; init; }
}
