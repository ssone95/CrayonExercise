using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Orders.Parts;

namespace Crayon.API.Domain.DTOs.Requests.Orders;

public class OrderServicesRequest
{
    public required Guid AccountId { get; init; }
    public required List<OrderLinePart> Lines { get; init; }
    
    [JsonIgnore]
    public CustomerAccountDetailsContextModel? UserContext { get; set; }
}