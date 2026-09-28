using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.CurrencyConversion;

public record ConvertCurrencyCommand(
    [property: Description("Código de la moneda de origen")]
    string FromCurrencyCode,
    [property: Description("Código de la moneda de destino")]
    string ToCurrencyCode,
    [property: Description("Monto a convertir")]
    decimal Amount
);

public record CurrencyConversionResult(
    [property: Description("Código de la moneda de origen")]
    string FromCurrency,
    [property: Description("Código de la moneda de destino")]
    string ToCurrency,
    [property: Description("Monto en la moneda de origen")]
    decimal OriginalAmount,
    [property: Description("Monto convertido en la moneda de destino")]
    decimal ConvertedAmount);

public class ConvertCurrencyCommandValidator : AbstractValidator<ConvertCurrencyCommand>
{
    public ConvertCurrencyCommandValidator()
    {
        RuleFor(x => x.FromCurrencyCode)
            .NotEmptyOrWhiteSpace("FromCurrencyCode es obligatorio.")
            .Length(3).WithMessage("El código de moneda origen debe tener exactamente 3 caracteres.");
        RuleFor(x => x.ToCurrencyCode)
            .NotEmptyOrWhiteSpace("ToCurrencyCode es obligatorio.")
            .Length(3).WithMessage("El código de moneda destino debe tener exactamente 3 caracteres.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount debe ser mayor a 0.");
    }
}

public class ConvertCurrencyCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<CurrencyConversionResult> Handle(ConvertCurrencyCommand request,
        CancellationToken cancellationToken)
    {
        var fromCode = request.FromCurrencyCode.Trim().ToUpperInvariant();
        var toCode = request.ToCurrencyCode.Trim().ToUpperInvariant();

        var fromTask = FetchCurrencyRateAsync(fromCode, cancellationToken);
        var toTask = FetchCurrencyRateAsync(toCode, cancellationToken);

        await Task.WhenAll(fromTask, toTask);

        var fromRate = await fromTask;
        var toRate = await toTask;

        if (!fromRate.HasValue)
            throw new NotFoundException($"Moneda origen '{request.FromCurrencyCode}' no encontrada.");
        if (!toRate.HasValue)
            throw new NotFoundException($"Moneda destino '{request.ToCurrencyCode}' no encontrada.");

        var baseAmount = request.Amount * fromRate.Value;
        var convertedAmount = baseAmount / toRate.Value;

        return new CurrencyConversionResult(
            fromCode,
            toCode,
            request.Amount,
            Math.Round(convertedAmount, 4)
        );
    }

    private async Task<decimal?> FetchCurrencyRateAsync(string code, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var currency = await context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, ct);

        return currency?.RateToBase;
    }
}
