using Crayon.API.Domain.DTOs.Responses.Base;
using Crayon.API.Domain.DTOs.Responses.External.CCP.Parts;

namespace Crayon.API.Domain.DTOs.Responses.External.CCP;

public class CCPOrderServicesResponse : BaseResponse<List<CCPSubmittedOrderLinePart>>
{
    public Guid OrderId { get; init; }
    public double TransactionFee { get; init; }
    public double Total => Data?.Sum(x => x.TotalPrice) ?? 0;
    public double TotalTax => Data?.Sum(x => x.TotalTax) ?? 0;
    public double GrandTotal => Total + TotalTax + TransactionFee;
}