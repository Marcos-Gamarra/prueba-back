using Microsoft.AspNetCore.Diagnostics;

namespace PruebaTecnicaBack.ExceptionHandlers;

public class GlobalFallbackExceptionHandler(ILogger<GlobalFallbackExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ocurrio un error inesperado en la aplicacion: {Message}", exception.Message);

        return await ProblemDetailsWriter.WriteAsync(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "Ocurrio un error inesperado en la aplicacion. Por favor, intente nuevamente mas tarde.",
            null,
            cancellationToken);
    }
}
