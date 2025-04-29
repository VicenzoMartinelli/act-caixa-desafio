using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Act.Caixa.Services.Lancamento.Features.Lancamentos.Queries
{
    public class FindLancamentosQuery : IRequest<IEnumerable<LancamentoViewModel>>
    {
        public Guid Id { get; set; }
    }

    public class FindLancamentosQueryHandler(CaixaDbContext dbContext) 
        : IRequestHandler<FindLancamentosQuery, IEnumerable<LancamentoViewModel>>
    {
        public async Task<IEnumerable<LancamentoViewModel>> Handle(FindLancamentosQuery request, CancellationToken cancellationToken)
        {
            return await dbContext.Lancamentos
                .Select(l => LancamentoViewModel.FromDomain(l))
                .ToListAsync(cancellationToken);
        }
    }
}
