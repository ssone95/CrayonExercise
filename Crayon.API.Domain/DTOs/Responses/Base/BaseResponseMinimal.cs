namespace Crayon.API.Domain.DTOs.Responses.Base;

public class BaseResponseMinimal
{
    public bool Success { get; init; }
    public string? Message { get; init; }
}