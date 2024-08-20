using System.Text.Json.Serialization;
using Crayon.API.Domain.Enums;
using Crayon.API.Domain.Formatters;

namespace Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;

public class OrderEntryModel
{
    public Guid OrderId { get; init; }
    
    public DateTime PostedOn { get; init; }
    
    public OrderStatus Status { get; init; }
    
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TransactionFee { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double Total { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalTax { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double GrandTotal { get; init; }
    
    public required List<OrderEntryLineModel> Lines { get; init; }
}