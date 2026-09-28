using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Application.Users.Dtos;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Users.Queries;

public record GetUserByIdQuery(
    [property: Description("ID del usuario a obtener")]
    int Id
);

public class GetUserByIdQueryHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new NotFoundException($"No se encontró un usuario con el ID {request.Id}.");
    }
}