using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace PruebaTecnicaBack.ExceptionHandlers;

public class DatabaseExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        return exception switch
        {
            UniqueConstraintException => await ProblemDetailsWriter.WriteAsync(
                httpContext,
                StatusCodes.Status409Conflict,
                "Unique Constraint Conflict",
                "Ya existe un registro con información duplicada en la base de datos.",
                null,
                cancellationToken),

            ReferenceConstraintException => await ProblemDetailsWriter.WriteAsync(
                httpContext,
                StatusCodes.Status409Conflict,
                "Reference Constraint Conflict",
                "No se pudo completar la operación debido a una restricción de clave foránea o referencia en la base de datos.",
                null,
                cancellationToken),

            CannotInsertNullException => await ProblemDetailsWriter.WriteAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "Uno o más campos obligatorios no fueron provistos en la base de datos.",
                null,
                cancellationToken),

            NumericOverflowException => await ProblemDetailsWriter.WriteAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "Un valor numérico excede los límites permitidos.",
                null,
                cancellationToken),

            _ => false
        };
    }
}
