using Crayon.API.Domain.DTOs.Requests.Internal.CCP.Parts;

namespace Crayon.API.Domain.DTOs.Requests.Internal.CCP;

public class CCPOrderServicesRequest
{
    public Guid OrderId { get; init; }
    public required List<CCPOrderLinePart> Lines { get; init; }
}