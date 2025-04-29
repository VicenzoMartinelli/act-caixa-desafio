namespace Act.Caixa.Domain.Entities;

public class ConsolidacaoDiaria
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateOnly Data { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; protected set; }
    public decimal TotalEntradas { get; set; }

    public decimal TotalSaidas { get; set; }

    // Controle de concorrência
    public uint Version { get; set; }

    protected ConsolidacaoDiaria()
    {
    }

    public ConsolidacaoDiaria(DateOnly data, decimal saldoInicial)
    {
        Data = data;

        SaldoInicial = saldoInicial;
    }

    public void CalcularSaldoFinal()
    {
        SaldoFinal = Math.Round(SaldoInicial + TotalEntradas + TotalSaidas, 2);
        AtualizadoEm = DateTime.UtcNow;
    }
}