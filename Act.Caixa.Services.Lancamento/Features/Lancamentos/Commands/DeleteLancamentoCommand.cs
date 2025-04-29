using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.BuildingBlocks.Shared.Models;
using Act.Caixa.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;

public class DeleteLancamentoCommand : IRequest<(bool ok, ICommandResult model)>
{
    public Guid Id { get; set; }
}

public class DeleteLancamentoCommandHandler(CaixaDbContext dbContext, IPublishEndpoint publishEndpoint) 
    : IRequestHandler<DeleteLancamentoCommand, (bool ok, ICommandResult model)>
{
    public async Task<(bool ok, ICommandResult model)> Handle(
        DeleteLancamentoCommand request, 
        CancellationToken cancellationToken
    )
    {
        var lancamento = await dbContext.Lancamentos
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lancamento is null)
        {
            return (false, FailedCommandResult.New("Lançamento não encontrado"));
        }

        dbContext.Lancamentos.Remove(lancamento);
        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new LancamentoDiaAtualizadoEvent
        {
            Data = DateOnly.FromDateTime(lancamento.CriadoEm.Date)
        }, cancellationToken);
        
        return (true, SuccessCommandResult.New());
    }
}