using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Users.Dtos;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Users.Queries;

public record GetUsersQuery(
    [property:
        Description(
            "Indica si se deben filtrar los usuarios por estado activo o inactivo. Si es null, no se aplica ningún filtro.")]
    bool? IsActive
);

public class GetUsersQueryHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Users.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        return await query
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive))
            .ToListAsync(cancellationToken);
    }
}