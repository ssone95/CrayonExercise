namespace Crayon.API.Domain.DTOs.Responses.External.Parts;

public class ListServicesResponseLine
{
    public Guid ServiceId { get; init; }
    public required string Name { get; init; }
    public double Price { get; init; }
    public bool IsAvailable { get; init; }
}