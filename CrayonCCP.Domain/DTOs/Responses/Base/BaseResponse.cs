namespace CrayonCCP.Domain.DTOs.Responses.Base;

public class BaseResponse<T> : BaseResponseMinimal
{
    public T? Data { get; init; }

    public BaseResponse(T data)
    {
        Data = data;
    }

    public BaseResponse()
    {
        Data = default(T);
    }
}