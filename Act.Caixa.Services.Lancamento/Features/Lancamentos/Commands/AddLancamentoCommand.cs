using System.ComponentModel.DataAnnotations;
using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.BuildingBlocks.Shared.Models;
using Act.Caixa.Domain.Entities;
using Act.Caixa.Domain.Events;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.ViewModels;
using MassTransit;
using MediatR;

namespace Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;

public class AddLancamentoCommand : IRequest<(bool ok, ICommandResult model)>
{
    [Required]
    public required string Descricao { get; set; }
    [Range(0 , double.MaxValue)]
    public decimal Valor { get; set; }
    public DateTime DataLancamento { get; set; }
    public TipoLancamentoEnum Tipo { get; set; }
}

public class AddLancamentoCommandHandler(CaixaDbContext dbContext, IPublishEndpoint publishEndpoint) 
    : IRequestHandler<AddLancamentoCommand, (bool ok, ICommandResult model)>
{
    public async Task<(bool ok, ICommandResult model)> Handle(
        AddLancamentoCommand request, 
        CancellationToken cancellationToken
    )
    {
        if (request.DataLancamento.Date > DateTime.UtcNow.Date)
        {
            return (false, FailedCommandResult.New("Data de lançamento não pode ser futura."));
        }
        
        var lancamento = new LancamentoCaixa
        {
            Descricao = request.Descricao,
            Valor = request.Tipo == TipoLancamentoEnum.Saida ? -request.Valor : request.Valor,
            TipoLancamento = request.Tipo,
            CriadoEm = request.DataLancamento
        };

        await dbContext.Lancamentos.AddAsync(lancamento, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new LancamentoDiaAtualizadoEvent
        {
            Data = DateOnly.FromDateTime(lancamento.CriadoEm) 
        }, cancellationToken);
        
        return (true, LancamentoViewModel.FromDomain(lancamento));
    }
}