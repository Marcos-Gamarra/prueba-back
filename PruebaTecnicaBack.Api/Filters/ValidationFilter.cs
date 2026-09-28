using FluentValidation;

namespace PruebaTecnicaBack.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            return TypedResults.Problem(
                detail: "El cuerpo de la solicitud es obligatorio y no puede estar vacío.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request");
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
            throw new InvalidOperationException($"No se encontró un validador registrado para el tipo {typeof(T).Name}.");

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}