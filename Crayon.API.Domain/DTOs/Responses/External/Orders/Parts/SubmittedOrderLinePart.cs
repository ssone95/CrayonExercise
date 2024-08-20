using System.Text.Json.Serialization;
using Crayon.API.Domain.Enums;
using Crayon.API.Domain.Formatters;

namespace Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;

public class SubmittedOrderLinePart
{
    public Guid ServiceId { get; init; }
    public required string ServiceName { get; set; }
    public int Quantity { get; init; }
    public double UnitPrice { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TaxPerUnitMultiplier { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double BaseTotalPrice => Quantity * UnitPrice;
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalTax => BaseTotalPrice * TaxPerUnitMultiplier;
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalPrice => BaseTotalPrice + TotalTax;
}