namespace Crayon.API.Domain.DTOs.Responses.External.CCP.Parts;

public class CCPServiceLine
{
    public Guid ServiceId { get; init; }
    public required string Name { get; init; }
    public double Price { get; init; }
    public bool IsAvailable { get; init; }
    public double TaxMultiplier { get; init; }
}