using System.Net;
using System.Security.Authentication;
using Crayon.API.Domain.DTOs.Responses.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Crayon.API.Infrastructure.Services.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            context.Request.Headers.Append("X-Trace-Id", Guid.NewGuid().ToString());
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response already began, can't overwrite anything!\nError: {ex}", exception);
            return;
        }
        _logger.LogWarning("An error occurred, trying to handle it...\nException: {ex}", exception);

        context.Response.StatusCode = GetStatusCodeForExType(exception, context.Response.StatusCode);
        var jsonResponse = GetResponseBodyForExType(exception);
        await context.Response.WriteAsJsonAsync(jsonResponse);
    }
    
    private int GetStatusCodeForExType(Exception exception, int originalStatusCode)
    {
        return exception switch
        {
            not null when exception is AuthenticationException => (int)HttpStatusCode.Unauthorized,
            not null when exception is UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            _ => originalStatusCode
        };
    }

    private BaseResponseMinimal GetResponseBodyForExType(Exception exception)
    {
        BaseResponseMinimal response = new()
        {
            Success = false,
            Message = exception switch
            {
                not null when exception is AuthenticationException => exception.Message,
                not null when exception is UnauthorizedAccessException => exception.Message,
                _ => "Unknown error, please try again!"
            }
        };
        return response;
    }
}