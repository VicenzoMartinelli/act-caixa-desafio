using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.Domain.Entities;
using Act.Caixa.Domain.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Act.Caixa.Consolidacao.Features.Consolidacao.Consumers;

public class AtualizaConsolidacaoDiariaConsumer(CaixaDbContext dbContext) : IConsumer<LancamentoDiaAtualizadoEvent>
{
    public async Task Consume(ConsumeContext<LancamentoDiaAtualizadoEvent> context)
    {
        var consolidacao = await dbContext.Consolidacoes
            .Where(c => c.Data == context.Message.Data)
            .FirstOrDefaultAsync();
        
        var dataReferencia = context.Message.Data.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var valoresPorTipoLancamentos = await dbContext.Lancamentos
            .Where(l => l.CriadoEm.Date == dataReferencia)
            .GroupBy(l => l.TipoLancamento)
            .Select(group => new
            {
                Tipo = group.Key,
                Valor = group.Sum(l => l.Valor)
            })
            .ToListAsync();
        var valorEntradas = valoresPorTipoLancamentos.SingleOrDefault(v => v.Tipo == TipoLancamentoEnum.Entrada)?.Valor ?? 0;
        var valorSaidas = valoresPorTipoLancamentos.SingleOrDefault(v => v.Tipo == TipoLancamentoEnum.Saida)?.Valor ?? 0;
        
        if (consolidacao is null)
        {
            var saldoFinalAnterior = await dbContext.Consolidacoes
                .Where(c => c.Data < context.Message.Data)
                .OrderByDescending(c => c.Data)
                .Select(c => new { c.SaldoFinal })
                .FirstOrDefaultAsync();

            consolidacao = new ConsolidacaoDiaria(
                data: context.Message.Data,
                saldoInicial: saldoFinalAnterior?.SaldoFinal ?? 0
            );
            dbContext.Add(consolidacao);
        }
        
        consolidacao.TotalEntradas = valorEntradas;
        consolidacao.TotalSaidas = valorSaidas;
        
        consolidacao.CalcularSaldoFinal();

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}