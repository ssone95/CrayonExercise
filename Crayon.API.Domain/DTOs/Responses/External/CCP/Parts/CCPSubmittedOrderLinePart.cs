namespace Crayon.API.Domain.DTOs.Responses.External.CCP.Parts;

public class CCPSubmittedOrderLinePart
{
    public Guid ServiceId { get; init; }
    public int Quantity { get; init; }
    public int UnitPrice { get; init; }
    public double TaxPerUnitMultiplier { get; init; }
    public double BaseTotalPrice => Quantity * UnitPrice;
    public double TotalTax => BaseTotalPrice * TaxPerUnitMultiplier;
    public double TotalPrice => BaseTotalPrice + TotalTax;
}