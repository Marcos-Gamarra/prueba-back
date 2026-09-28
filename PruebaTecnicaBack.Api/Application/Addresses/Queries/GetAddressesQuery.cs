using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Addresses.Dtos;
using PruebaTecnicaBack.Application.Exceptions;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Addresses.Queries;

public record GetAddressesQuery(int UserId);

public class GetAddressesQueryHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var userExists = await context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException($"Usuario con Id {request.UserId} no encontrado.");
        }

        return await context.Addresses
            .Where(a => a.UserId == request.UserId)
            .Select(a => new AddressDto(a.Id, a.UserId, a.Street, a.City, a.Country, a.ZipCode))
            .ToListAsync(cancellationToken);
    }
}
