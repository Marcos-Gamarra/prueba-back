using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Addresses.Commands;

public record UpdateAddressCommand(
    [property: Description("Calle de la dirección")]
    string Street,
    [property: Description("Ciudad de la dirección")]
    string City,
    [property: Description("País de la dirección")]
    string Country,
    [property: Description("Código postal de la dirección (opcional)")]
    string? ZipCode
);

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Street).NotEmptyOrWhiteSpace("La calle es obligatoria.");
        RuleFor(x => x.City).NotEmptyOrWhiteSpace("La ciudad es obligatoria.");
        RuleFor(x => x.Country).NotEmptyOrWhiteSpace("El país es obligatorio.");
    }
}

public class UpdateAddressCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task Handle(int addressId, UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var affectedRows = await context.Addresses
            .Where(a => a.Id == addressId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(a => a.Street, request.Street.Trim())
                    .SetProperty(a => a.City, request.City.Trim())
                    .SetProperty(a => a.Country, request.Country.Trim())
                    .SetProperty(a => a.ZipCode, request.ZipCode?.Trim()), 
                cancellationToken);

        if (affectedRows == 0)
        {
            throw new NotFoundException($"Dirección con Id {addressId} no encontrada.");
        }
    }
}
