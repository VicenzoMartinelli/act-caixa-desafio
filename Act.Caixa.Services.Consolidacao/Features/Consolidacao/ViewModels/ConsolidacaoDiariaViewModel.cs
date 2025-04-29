using Act.Caixa.Domain.Entities;

namespace Act.Caixa.Consolidacao.Features.Consolidacao.ViewModels;

public record ConsolidacaoDiariaViewModel
{
    public Guid Id { get; set; }
    public DateOnly Data { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    
    
    public static ConsolidacaoDiariaViewModel FromDomain(ConsolidacaoDiaria consolidacao)
    {
        return new ConsolidacaoDiariaViewModel
        {
            Id = consolidacao.Id,
            Data = consolidacao.Data,
            SaldoInicial = consolidacao.SaldoInicial,
            SaldoFinal = consolidacao.SaldoFinal,
            TotalEntradas = consolidacao.TotalEntradas,
            TotalSaidas = consolidacao.TotalSaidas
        };
    }
}