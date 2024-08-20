namespace Crayon.API.Domain.DTOs.Requests.Internal.CCP.Parts;

public class CCPOrderLinePart
{
    public Guid Id { get; init; }
    public int Quantity { get; init; }
    public required string Name { get; set; }
}