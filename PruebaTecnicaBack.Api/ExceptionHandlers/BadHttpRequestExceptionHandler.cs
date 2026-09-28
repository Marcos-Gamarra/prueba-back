using Microsoft.AspNetCore.Diagnostics;

namespace PruebaTecnicaBack.ExceptionHandlers;

public class BadHttpRequestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequestEx)
            return false;

        return await ProblemDetailsWriter.WriteAsync(
            httpContext,
            badRequestEx.StatusCode,
            "Bad Request",
            badRequestEx.Message,
            null,
            cancellationToken);
    }
}
