using Microsoft.Extensions.DependencyInjection;

namespace Act.Caixa.BuildingBlocks.Infra.Db;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services
    )
    {
        services.AddScoped<CaixaDbContext>();

        return services;
    }
}