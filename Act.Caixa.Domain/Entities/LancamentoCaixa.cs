namespace Act.Caixa.Domain.Entities;

public class LancamentoCaixa
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Descricao { get; set; }
    public TipoLancamentoEnum TipoLancamento { get; set; }
    public decimal Valor { get; set; }
    public DateTime CriadoEm { get; set; }
}