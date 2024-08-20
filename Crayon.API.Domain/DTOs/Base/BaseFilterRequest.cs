namespace Crayon.API.Domain.DTOs.Base;

public class BaseFilterRequest
{
    public string? FilterString { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 10;
}