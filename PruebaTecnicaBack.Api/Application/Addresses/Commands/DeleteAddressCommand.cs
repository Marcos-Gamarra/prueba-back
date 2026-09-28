using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Addresses.Commands;

public record DeleteAddressCommand(
    [property: Description("Id de la dirección a eliminar")]
    int Id
);

public class DeleteAddressCommandHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var deletedRows = await context.Addresses
            .Where(a => a.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);       

        if (deletedRows == 0)
        {
            throw new NotFoundException($"Dirección con Id {request.Id} no encontrada.");
        }
    }
}