namespace Crayon.API.Domain.DTOs.Responses.Base;

public class BaseFilterResponse<T> : BaseResponse<T>
{
    public int CurrentPage { get; init; }
    public int ItemsPerPage { get; init; }
    public int TotalPages { get; init; }
}