using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.BuildingBlocks.Shared.Models;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.ViewModels;
using MediatR;

namespace Act.Caixa.Services.Lancamento.Features.Lancamentos.Queries
{
    public class FindLancamentoByIdQuery : IRequest<ICommandResult>
    {
        public Guid Id { get; set; }
    }

    public class FindLancamentoByIdQueryHandler(CaixaDbContext dbContext) : IRequestHandler<FindLancamentoByIdQuery, ICommandResult>
    {
        public async Task<ICommandResult> Handle(
            FindLancamentoByIdQuery request,
            CancellationToken cancellationToken)
        {
            var lancamento = await dbContext.Lancamentos
                .FindAsync(request.Id, cancellationToken);

            if (lancamento is null)
            {
                return FailedCommandResult.New("Lançamento não encontrado");
            }

            return LancamentoViewModel.FromDomain(lancamento);
        }
    }
}
