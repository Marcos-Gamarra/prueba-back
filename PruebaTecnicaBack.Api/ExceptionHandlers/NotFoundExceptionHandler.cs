using Microsoft.AspNetCore.Diagnostics;
using PruebaTecnicaBack.Application.Exceptions;

namespace PruebaTecnicaBack.ExceptionHandlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        return await ProblemDetailsWriter.WriteAsync(
            httpContext,
            StatusCodes.Status404NotFound,
            "Not Found",
            notFoundException.Message,
            null,
            cancellationToken);
    }
}
