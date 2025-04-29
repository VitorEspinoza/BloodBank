using System.Net;
using BloodBank.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BloodBank.API.ExceptionHandler;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        
        switch (exception)
        {
            case AddressServiceUnavailableException ex:
                await HandleAddressServiceException(context, ex);
                return true;

            default:
                await HandleUnknownException(context, exception);
                return true;
        }
        return true;
    }
    
    private async Task HandleAddressServiceException(
        HttpContext context,
        AddressServiceUnavailableException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "External Service Unavailable",
            Detail = exception.Message,
            Status = (int)HttpStatusCode.ServiceUnavailable,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            Extensions = { 
                ["service"] = "AddressService",
                ["retryAfter"] = "60"
            }
        };

        context.Response.Headers.RetryAfter = "60";
        await WriteProblemDetails(context, problemDetails);
    }
    
    private async Task HandleUnknownException(
        HttpContext context,
        Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred",
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() 
                ? exception.ToString() 
                : "Please try again later or contact support.",
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        await WriteProblemDetails(context, problemDetails);
    }
    
    private async Task WriteProblemDetails(
        HttpContext context,
        ProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status.Value;
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problemDetails
        });
    }
}