namespace Crayon.API.Domain.DTOs.Requests.Orders.Parts;

public class OrderLinePart
{
    public Guid ServiceId { get; init; }
    public required string Name { get; init; }
    public int Quantity { get; init; }
}