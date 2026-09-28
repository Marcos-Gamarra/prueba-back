using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using PruebaTecnicaBack.Options;

namespace PruebaTecnicaBack.Filters;

public class ApiKeyFilter(IOptions<ApiKeySecurityOptions> apiKeyOptions) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var expectedKey = apiKeyOptions.Value.SecretKey;
        var headerName = apiKeyOptions.Value.HeaderName;

        if (string.IsNullOrEmpty(expectedKey) ||
            !context.HttpContext.Request.Headers.TryGetValue(headerName, out var providedKey) ||
            !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedKey),
                Encoding.UTF8.GetBytes(providedKey.ToString())))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}