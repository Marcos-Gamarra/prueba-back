using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using PruebaTecnicaBack.Options;

namespace PruebaTecnicaBack.OpenApi;

public class ApiKeySecuritySchemeTransformer(IOptions<ApiKeySecurityOptions> apiKeyOptions)
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["ApiKey"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = apiKeyOptions.Value.HeaderName,
                Description = "API Key requerida para acceder a los endpoints."
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        document.Paths.Values
            .SelectMany(path => path.Operations ?? [])
            .ToList()
            .ForEach(operation =>
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
                });
            });
        
        return Task.CompletedTask;
    }
}