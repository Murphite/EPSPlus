﻿using System.Net;
using Newtonsoft.Json;

namespace EPSPlus.API.Presentation.Middlewares;

public class ExceptionMiddleware : IExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly bool _isProdEnv;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment hostingEnvironment)
    {
        _next = next;
        _logger = logger;
        _isProdEnv = hostingEnvironment.IsProduction();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await HandleException(context, ex, "Invalid argument.", HttpStatusCode.BadRequest);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleException(context, ex, "Unauthorized access.", HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex, "An unexpected error occurred.", HttpStatusCode.InternalServerError);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex, string errorMessage, HttpStatusCode statusCode)
    {
        // Log the exception with detailed information
        _logger.LogError(ex, ex.Message);

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode; // Set the response status code explicitly

        // Create error response
        var errorDetails = _isProdEnv
            ? errorMessage
            : $"{errorMessage} | Exception: {ex.Message}";  // Include exception message only in non-production

        var errorResponse = new ErrorResponse
        {
            ResponseCode = ((int)statusCode).ToString(),
            ResponseMessage = errorMessage,
            ResponseDescription = errorDetails
        };

        var result = JsonConvert.SerializeObject(ServerResponseExtensions.Failure<object>(
            errorResponse, (int)statusCode));

        await response.WriteAsync(result);
    }


    
}
