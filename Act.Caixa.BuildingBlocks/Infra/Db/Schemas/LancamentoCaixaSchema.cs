using Act.Caixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Act.Caixa.BuildingBlocks.Infra.Db.Schemas;

public class LancamentoCaixaSchema : IEntityTypeConfiguration<LancamentoCaixa>
{
    public void Configure(EntityTypeBuilder<LancamentoCaixa> builder)
    {
        builder.ToTable("lancamentos");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Id)
            .HasColumnName("id");
            
        builder.Property(l => l.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(l => l.TipoLancamento)
            .HasColumnName("tipo_lancamento")
            .IsRequired();
            
        builder.Property(l => l.Valor)
            .HasColumnName("valor")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(l => l.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();
        
        builder.HasIndex(l => l.CriadoEm);
    }
}