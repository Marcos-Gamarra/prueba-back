using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PruebaTecnicaBack;
using PruebaTecnicaBack.Endpoints;
using PruebaTecnicaBack.Filters;
using PruebaTecnicaBack.Infrastructure.Persistence;
using PruebaTecnicaBack.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    var apiKey = app.Services.GetRequiredService<IOptions<ApiKeySecurityOptions>>().Value.SecretKey;

    app.MapScalarApiReference(options => options
        .AddPreferredSecuritySchemes("ApiKey")
        .AddApiKeyAuthentication("ApiKey", key => { key.Value = apiKey; }));

    app.MapGet("/swagger", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
    app.MapGet("/swagger/index.html", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

var dbFactory = app.Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
await using (var dbContext = await dbFactory.CreateDbContextAsync())
{
    await dbContext.Database.OpenConnectionAsync();
    await dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

var protectedEndpoints = app.MapGroup("/").AddEndpointFilter<ApiKeyFilter>();
protectedEndpoints.MapUserEndpoints();
protectedEndpoints.MapAddressEndpoints();
protectedEndpoints.MapCurrencyEndpoints();

await app.RunAsync();