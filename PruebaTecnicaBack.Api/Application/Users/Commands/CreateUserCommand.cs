using System.ComponentModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Common.Validators;
using PruebaTecnicaBack.Application.Interfaces;
using PruebaTecnicaBack.Application.Users.Dtos;
using PruebaTecnicaBack.Domain.Entities;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Users.Commands;

public record CreateUserCommand(
    [property: Description("Nombre del usuario")]
    string Name,
    [property: Description("Dirección de correo electrónico del usuario")]
    string Email,
    [property: Description("Contraseña del usuario (opcional).")]
    string? Password
);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace("Nombre es requerido.");
        RuleFor(x => x.Email).ValidCustomEmail();
        RuleFor(x => x.Password)
            .ValidCustomPassword()
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}

public class CreateUserCommandHandler(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    IPasswordHasher passwordHasher)
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            IsActive = true,
            PasswordHash = !string.IsNullOrWhiteSpace(request.Password)
                ? passwordHasher.Hash(request.Password)
                : null
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Name, user.Email, user.IsActive);
    }
}