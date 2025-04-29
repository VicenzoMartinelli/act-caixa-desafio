using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.Consolidacao.Features.Consolidacao.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Act.Caixa.Consolidacao.Features.Consolidacao.Queries;

public class FindConsolidacoesQuery : IRequest<IEnumerable<ConsolidacaoDiariaViewModel>>
{
    public DateOnly? FiltroData { get; set; }
}

public class FindConsolidacoesQueryHandler(CaixaDbContext dbContext, IMemoryCache cache) 
    : IRequestHandler<FindConsolidacoesQuery, IEnumerable<ConsolidacaoDiariaViewModel>>
{
    public async Task<IEnumerable<ConsolidacaoDiariaViewModel>> Handle(
        FindConsolidacoesQuery request, 
        CancellationToken cancellationToken
    )
    {
        var cacheKey = request.FiltroData.HasValue
            ? $"consolidacoes_{request.FiltroData:yyyy-MM-dd}"
            : "consolidacoes_todas";

        return (await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            
            var query = dbContext.Consolidacoes.AsQueryable();
            
            if (request.FiltroData.HasValue)
                query = query.Where(x => x.Data == request.FiltroData.Value);

            var lista =  await query
                .Select(c => ConsolidacaoDiariaViewModel.FromDomain(c))
                .ToListAsync(cancellationToken);

            return lista;
        }))!;

    }
}