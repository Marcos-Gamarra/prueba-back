using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Addresses.Dtos;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Domain.Entities;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Addresses.Commands;

public record CreateAddressCommand(
    [property: Description("Calle de la dirección")]
    string Street,
    [property: Description("Ciudad de la dirección")]
    string City,
    [property: Description("País de la dirección")]
    string Country,
    [property: Description("Código postal de la dirección (opcional)")]
    string? ZipCode
);

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Street).NotEmptyOrWhiteSpace("La calle es obligatoria.");
        RuleFor(x => x.City).NotEmptyOrWhiteSpace("La ciudad es obligatoria.");
        RuleFor(x => x.Country).NotEmptyOrWhiteSpace("El país es obligatorio.");
    }
}

public class CreateAddressCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<AddressDto> Handle(int userId, CreateAddressCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var userExists = await context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException($"Usuario con Id {userId} no encontrado.");
        }

        var address = new Address
        {
            UserId = userId,
            Street = request.Street.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
            ZipCode = request.ZipCode?.Trim()
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(cancellationToken);

        return new AddressDto(
            address.Id,
            address.UserId,
            address.Street,
            address.City,
            address.Country,
            address.ZipCode
        );
    }
}