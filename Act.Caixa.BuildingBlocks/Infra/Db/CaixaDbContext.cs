using Act.Caixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Act.Caixa.BuildingBlocks.Infra.Db;

public class CaixaDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<LancamentoCaixa> Lancamentos { get; set; }
    public DbSet<ConsolidacaoDiaria> Consolidacoes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(configuration["db_connection_string"]);
}