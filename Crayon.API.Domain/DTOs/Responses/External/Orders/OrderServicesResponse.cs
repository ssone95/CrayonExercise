using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Responses.Base;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;
using Crayon.API.Domain.Formatters;

namespace Crayon.API.Domain.DTOs.Responses.External.Orders;

public class OrderServicesResponse : BaseResponse<List<SubmittedOrderLinePart>>
{
    public Guid OrderId { get; init; }
    
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TransactionFee { get; init; }
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double Total => Data?.Sum(x => x.BaseTotalPrice) ?? 0;
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double TotalTax => Data?.Sum(x => x.TotalTax) ?? 0;
    [JsonConverter(typeof(DoubleLimitDecimalPlacesConverter))]
    public double GrandTotal => Total + TotalTax + TransactionFee;
}