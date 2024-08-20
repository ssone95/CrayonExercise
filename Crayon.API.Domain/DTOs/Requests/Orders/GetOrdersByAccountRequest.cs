using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Requests.Common;

namespace Crayon.API.Domain.DTOs.Requests.Orders;

public class GetOrdersByAccountRequest
{
    public Guid AccountId { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 10;
    [JsonIgnore]
    public required CustomerAccountDetailsContextModel UserContext { get; init; }
}