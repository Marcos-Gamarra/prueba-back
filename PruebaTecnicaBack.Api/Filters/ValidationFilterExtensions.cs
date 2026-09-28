namespace PruebaTecnicaBack.Filters;

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder AddValidation<T>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}