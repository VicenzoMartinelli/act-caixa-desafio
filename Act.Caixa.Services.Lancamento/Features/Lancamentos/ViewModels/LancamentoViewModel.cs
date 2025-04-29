using Act.Caixa.BuildingBlocks.Shared.Models;
using Act.Caixa.Domain.Entities;

namespace Act.Caixa.Services.Lancamento.Features.Lancamentos.ViewModels;

public record LancamentoViewModel : ICommandResult
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public TipoLancamentoEnum TipoLancamento { get; set; }
    public decimal Valor { get; set; }
    public DateTime CriadoEm { get; set; }

    public LancamentoViewModel()
    {
        
    }

    public static LancamentoViewModel FromDomain(LancamentoCaixa lancamento)
    {
        return new LancamentoViewModel
        {
            Id = lancamento.Id,
            Descricao = lancamento.Descricao,
            TipoLancamento = lancamento.TipoLancamento,
            Valor = lancamento.Valor,
            CriadoEm = lancamento.CriadoEm
        };
    }

    public Guid GetId() => Id;
}