using System.Text.Json.Serialization;
using Crayon.API.Domain.Enums;
using Crayon.API.Domain.Formatters;

namespace Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;

public class OrderEntryLineModel
{
    public Guid ServiceId { get; init; }
    public required string ServiceName { get; init; }
    public int Quantity { get; init; }
    public double UnitPrice { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TaxPerUnitMultiplier { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double BaseTotalPrice { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalTax { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalPrice { get; init; }
    public EntityStatus LicenseStatus { get; init; }
    
    public DateTime ExpiresOn { get; init; }
}