using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Users.Commands;

public record DeleteUserCommand(
    [property: Description("Identificador único del usuario")]
    int Id
);

public class DeleteUserCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var rowsDeleted = await context.Users
            .Where(u => u.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
        {
            throw new NotFoundException($"No se encontró un usuario con el ID {request.Id}.");
        }
    }
}