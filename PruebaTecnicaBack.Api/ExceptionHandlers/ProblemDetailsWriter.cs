using Microsoft.AspNetCore.Mvc;

namespace PruebaTecnicaBack.ExceptionHandlers;

public static class ProblemDetailsWriter
{
    public static async Task<bool> WriteAsync(
        HttpContext context,
        int statusCode,
        string title,
        string? detail,
        object? errors,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{statusCode}",
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (errors is not null)
        {
            problemDetails.Extensions.Add("errors", errors);
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
