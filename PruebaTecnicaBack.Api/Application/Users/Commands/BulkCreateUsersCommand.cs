using System.Collections.Concurrent;
using System.ComponentModel;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PruebaTecnicaBack.Application.Interfaces;
using PruebaTecnicaBack.Domain.Entities;
using PruebaTecnicaBack.Infrastructure.Persistence;
using PruebaTecnicaBack.Options;

namespace PruebaTecnicaBack.Application.Users.Commands;

public record BulkCreateUsersCommand(
    [property: Description("Lista de usuarios a crear")]
    List<CreateUserCommand?> Users
);

public record BulkItemError(int Index, string? Email, string Reason);

public record BulkCreateUsersResult(int Total, int Successful, int Failed, List<BulkItemError> Errors);

public class BulkCreateUsersCommandValidator : AbstractValidator<BulkCreateUsersCommand>
{
    public BulkCreateUsersCommandValidator(IOptions<BulkUsersOptions> options)
    {
        var maxBatchSize = options.Value.MaxBatchSize;
        RuleFor(x => x.Users)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Debe incluir al menos un usuario.")
            .Must(users => users.Count <= maxBatchSize)
            .WithMessage($"No se permiten más de {maxBatchSize} usuarios por solicitud.");
    }
}

public class BulkCreateUsersCommandHandler(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    IPasswordHasher passwordHasher,
    IValidator<CreateUserCommand> itemValidator,
    IOptions<BulkUsersOptions> bulkUsersOptions,
    ILogger<BulkCreateUsersCommandHandler> logger)
{
    public async Task<BulkCreateUsersResult> Handle(BulkCreateUsersCommand request, CancellationToken cancellationToken)
    {
        var errors = new ConcurrentBag<BulkItemError>();
        var successful = 0;

        var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var work = new List<(int Index, CreateUserCommand Item)>(request.Users.Count);

        for (var i = 0; i < request.Users.Count; i++)
        {
            var item = request.Users[i];
            if (item is null)
            {
                errors.Add(new BulkItemError(i, null, "Se omitió un elemento nulo en la lista de usuarios."));
                continue;
            }

            var validation = await itemValidator.ValidateAsync(item, cancellationToken);
            if (!validation.IsValid)
            {
                var reason = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
                errors.Add(new BulkItemError(i, item.Email, reason));
                continue;
            }

            var normalizedEmail = item.Email.Trim().ToLowerInvariant();
            if (!seenEmails.Add(normalizedEmail))
            {
                errors.Add(new BulkItemError(i, normalizedEmail, "Email duplicado dentro de la solicitud."));
                continue;
            }

            work.Add((i, item with { Email = normalizedEmail }));
        }

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = bulkUsersOptions.Value.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(work, parallelOptions, async (entry, ct) =>
        {
            var (index, item) = entry;
            await using var context = await contextFactory.CreateDbContextAsync(ct);

            var user = new User
            {
                Name = item.Name.Trim(),
                Email = item.Email,
                IsActive = true,
                PasswordHash = !string.IsNullOrWhiteSpace(item.Password) ? passwordHasher.Hash(item.Password) : null
            };

            context.Users.Add(user);

            try
            {
                await context.SaveChangesAsync(ct);
                Interlocked.Increment(ref successful);
            }
            catch (UniqueConstraintException)
            {
                errors.Add(new BulkItemError(index, item.Email,
                    $"Ya existe un usuario con el correo electrónico '{item.Email}'."));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Fallo al insertar el elemento {Index} del lote", index);
                errors.Add(new BulkItemError(index, item.Email, "Error del servidor al insertar el usuario."));
            }
        });

        var orderedErrors = errors.OrderBy(e => e.Index).ToList();

        logger.LogInformation(
            "Bulk insert completado. Total: {Total}, Insertados: {Successful}, Errores: {ErrorCount}",
            request.Users.Count, successful, orderedErrors.Count);

        return new BulkCreateUsersResult(request.Users.Count, successful, orderedErrors.Count, orderedErrors);
    }
}