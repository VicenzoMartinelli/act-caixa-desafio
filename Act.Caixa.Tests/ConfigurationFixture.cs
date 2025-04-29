using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.Consolidacao.Features.Consolidacao.Consumers;
using Act.Caixa.Consolidacao.Features.Consolidacao.Queries;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;
using DotNet.Testcontainers.Builders;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Act.Caixa.Tests;

public class ConfigurationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(5432))
        .WithDatabase("act_caixa_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();


    public required IServiceProvider ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.DisposeAsync().AsTask(),
            _rabbitMq.DisposeAsync().AsTask()
        );
    }

    public async ValueTask InitializeAsync()
    {
        Console.WriteLine("=== InitializeAsync ===");
        
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();

        var memorySettings = new Dictionary<string, string>
        {
            ["db_connection_string"] = _postgres.GetConnectionString(),
            ["rabbitmq_host"] = _rabbitMq.Hostname,
            ["rabbitmq_user"] = "guest",
            ["rabbitmq_password"] = "guest"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(memorySettings!)
            .Build();
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddOptions();
        services.AddDatabaseConfiguration();
        services.AddMassTransit(bus =>
        {
            bus.AddConsumersFromNamespaceContaining<AtualizaConsolidacaoDiariaConsumer>();

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["rabbitmq_host"], "/", h =>
                {
                    h.Username(configuration["rabbitmq_user"]!);
                    h.Password(configuration["rabbitmq_user"]!);
                });

                cfg.ConfigureEndpoints(context);
            });

            bus.AddMassTransitTestHarness();
            bus.AddMassTransitTextWriterLogger();
        });
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddLancamentoCommand>();
            cfg.RegisterServicesFromAssemblyContaining<FindConsolidacoesQuery>();
        });

        ServiceProvider = services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CaixaDbContext>();
 
        await db.Database.MigrateAsync();
    }
}