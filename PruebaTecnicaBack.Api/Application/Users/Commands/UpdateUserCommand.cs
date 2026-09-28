using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Users.Commands;

public record UpdateUserCommand(
    [property: Description("Nombre del usuario")]
    string Name,
    [property: Description("Dirección de correo electrónico del usuario")]
    string Email,
    [property: Description("Indica si el usuario está activo")]
    bool IsActive
);

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace("El nombre es requerido.");
        RuleFor(x => x.Email).ValidCustomEmail();
    }
}

public class UpdateUserCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task Handle(int id, UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var trimmedName = request.Name.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var rowsUpdated = await context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Name, trimmedName)
                    .SetProperty(u => u.Email, normalizedEmail)
                    .SetProperty(u => u.IsActive, request.IsActive),
                cancellationToken);

        if (rowsUpdated == 0)
            throw new NotFoundException($"No se encontró un usuario con el ID {id}.");
    }
}