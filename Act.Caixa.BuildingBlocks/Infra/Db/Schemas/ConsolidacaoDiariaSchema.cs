using Act.Caixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Act.Caixa.BuildingBlocks.Infra.Db.Schemas;

public class ConsolidacaoDiariaSchema : IEntityTypeConfiguration<ConsolidacaoDiaria>
{
    public void Configure(EntityTypeBuilder<ConsolidacaoDiaria> builder)
    {
        builder.ToTable("consolidacoes_diarias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Data)
            .HasColumnName("data")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .IsRequired();

        builder.Property(c => c.SaldoInicial)
            .HasColumnName("saldo_inicial")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.SaldoFinal)
            .HasColumnName("saldo_final")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TotalEntradas)
            .HasColumnName("total_entradas")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TotalSaidas)
            .HasColumnName("total_saidas")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.HasIndex(c => c.Data)
            .IsUnique();
        
        builder.Property(b => b.Version)
            .IsRowVersion();
    }
}