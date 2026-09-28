using Microsoft.AspNetCore.Http.HttpResults;
using PruebaTecnicaBack.Application.Currencies.Commands;
using PruebaTecnicaBack.Application.Currencies.Dtos;
using PruebaTecnicaBack.Application.Currencies.Queries;
using PruebaTecnicaBack.Application.CurrencyConversion;
using PruebaTecnicaBack.Filters;

namespace PruebaTecnicaBack.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/currencies").WithTags("Monedas");

        group.MapGet("/", GetCurrencies);

        group.MapPost("/", CreateCurrency)
            .AddValidation<CreateCurrencyCommand>();

        var convertGroup = app.MapGroup("/currency").WithTags("Conversión de Monedas");
        
        convertGroup.MapPost("/convert", ConvertCurrency)
            .AddValidation<ConvertCurrencyCommand>();
    }

    private static async Task<Ok<List<CurrencyDto>>> GetCurrencies(
        GetCurrenciesQueryHandler handler,
        CancellationToken ct)
    {
        var currencies = await handler.Handle(new GetCurrenciesQuery(), ct);
        return TypedResults.Ok(currencies);
    }

    private static async Task<Created<CurrencyDto>> CreateCurrency(
        CreateCurrencyCommand command,
        CreateCurrencyCommandHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(command, ct);
        return TypedResults.Created($"/currencies", res);
    }

    private static async Task<Ok<CurrencyConversionResult>> ConvertCurrency(
        ConvertCurrencyCommand command,
        ConvertCurrencyCommandHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(command, ct);
        return TypedResults.Ok(res);
    }
}
