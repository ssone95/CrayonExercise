using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using CrayonCCP.Domain.DTOs.Responses.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CrayonCCP.Infrastructure.Services.Middlewares;

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
        
        switch (exception)
        {
            case {} when exception is AuthenticationException:
            {
                var jsonResponse = BuildErrorResponse(exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(jsonResponse);
                break;
            }
            case {} when exception is UnauthorizedAccessException:
            {
                var jsonResponse = BuildErrorResponse(exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(jsonResponse);
                break;
            }
            default:
            {
                _logger.LogError("Couldn't recover from the following error: {ex}", exception);
                var jsonResponse = BuildErrorResponse("Unknown error, please try again!");
                await context.Response.WriteAsJsonAsync(jsonResponse);
                break;
            }
        }
    }

    private BaseResponseMinimal BuildErrorResponse(string errorMessage)
    {
        var jsonResponse = new BaseResponseMinimal()
        {
            Success = false,
            Message = errorMessage
        };
        return jsonResponse;
    }
}