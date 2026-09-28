using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Application.Currencies.Dtos;
using PruebaTecnicaBack.Infrastructure.Persistence;

namespace PruebaTecnicaBack.Application.Currencies.Queries;

public record GetCurrenciesQuery();

public class GetCurrenciesQueryHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<List<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Currencies
            .Select(c => new CurrencyDto(c.Id, c.Code, c.Name, c.RateToBase))
            .ToListAsync(cancellationToken);
    }
}
