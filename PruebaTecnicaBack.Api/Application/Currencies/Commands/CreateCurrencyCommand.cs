using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Application.Currencies.Dtos;
using PruebaTecnicaBack.Domain.Entities;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Currencies.Commands;

public record CreateCurrencyCommand(
    [property: Description("Código de la moneda")]
    string Code,
    [property: Description("Nombre de la moneda")]
    string Name,
    [property: Description("Tasa de conversión a la moneda base")]
    decimal RateToBase
);

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmptyOrWhiteSpace("Code es obligatorio.")
            .Length(3).WithMessage("El código de moneda debe tener exactamente 3 caracteres.");
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace("Name es obligatorio.");
        RuleFor(x => x.RateToBase).GreaterThan(0).WithMessage("RateToBase debe ser mayor a 0.");
    }
}

public class CreateCurrencyCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<CurrencyDto> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var currency = new Currency
        {
            Code = normalizedCode,
            Name = request.Name.Trim(),
            RateToBase = request.RateToBase
        };

        context.Currencies.Add(currency);
        await context.SaveChangesAsync(cancellationToken);

        return new CurrencyDto(currency.Id, currency.Code, currency.Name, currency.RateToBase);
    }
}