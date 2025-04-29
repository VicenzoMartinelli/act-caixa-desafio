using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.Consolidacao.Features.Consolidacao.Consumers;
using Act.Caixa.Domain.Entities;
using Act.Caixa.Domain.Events;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Act.Caixa.Tests;

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public sealed class ConsolidacaoIntegrationTest(ConfigurationFixture configuration)
    : IClassFixture<ConfigurationFixture>
{
    [Fact(DisplayName = "Deveria cadastrar entrada")]
    public async Task Deveria_Conseguir_Cadastrar_Entrada()
    {
        await configuration.RunInScope(async (sp) =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<CaixaDbContext>();
            var testHarness = sp.GetRequiredService<ITestHarness>();

            var result = await mediator.Send(new AddLancamentoCommand
            {
                Descricao = "Entrada 1",
                Tipo = TipoLancamentoEnum.Entrada,
                Valor = 300,
                DataLancamento = new DateTime(2025, 04, 26, 12, 0, 0, DateTimeKind.Utc)
            }, TestContext.Current.CancellationToken);

            Assert.True(result.ok);

            var lancamento = await dbContext.Lancamentos
                .FirstOrDefaultAsync(l => l.Id == result.model.GetId(),
                    cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(lancamento);
            Assert.Equal(300, lancamento.Valor);
            Assert.True(
                await testHarness.Published.Any<LancamentoDiaAtualizadoEvent>(TestContext.Current.CancellationToken));
        });
    }

    [Fact(DisplayName = "Deveria cadastrar saída")]
    public async Task Deveria_Conseguir_Cadastrar_Saida()
    {
        await configuration.RunInScope(async (sp) =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<CaixaDbContext>();
            var testHarness = sp.GetRequiredService<ITestHarness>();

            var result = await mediator.Send(new AddLancamentoCommand
            {
                Descricao = "Saida 1",
                Tipo = TipoLancamentoEnum.Saida,
                Valor = 250,
                DataLancamento = new DateTime(2025, 04, 26, 13, 0, 0, DateTimeKind.Utc)
            }, TestContext.Current.CancellationToken);

            Assert.True(result.ok);

            var lancamento = await dbContext.Lancamentos
                .FirstOrDefaultAsync(l => l.Id == result.model.GetId(),
                    cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(lancamento);
            Assert.Equal(-250, lancamento.Valor);
            Assert.True(
                await testHarness.Published.Any<LancamentoDiaAtualizadoEvent>(TestContext.Current.CancellationToken));
        });
    }

    [Fact]
    public async Task Deveria_Conseguir_Remover_Lançamento()
    {
        await configuration.RunInScope(async (sp) =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var dbContext = sp.GetRequiredService<CaixaDbContext>();
            var testHarness = sp.GetRequiredService<ITestHarness>();

            var lancamentoRemocao = new LancamentoCaixa
            {
                Descricao = "Saida 1",
                TipoLancamento = TipoLancamentoEnum.Saida,
                Valor = 250,
                CriadoEm = new DateTime(2025, 04, 26, 13, 0, 0, DateTimeKind.Utc)
            };
            dbContext.Lancamentos.Add(lancamentoRemocao);
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var result = await mediator.Send(new DeleteLancamentoCommand
            {
                Id = lancamentoRemocao.Id
            }, TestContext.Current.CancellationToken);

            Assert.True(result.ok);
            Console.WriteLine(result.model);

            var lancamentoExisteNoDb = await dbContext.Lancamentos
                .AnyAsync(l => l.Id == result.model.GetId(), cancellationToken: TestContext.Current.CancellationToken);

            Assert.False(lancamentoExisteNoDb);
            Assert.True(
                await testHarness.Published.Any<LancamentoDiaAtualizadoEvent>(TestContext.Current.CancellationToken));
        });
    }

    [Fact(DisplayName = "Deveria consolidar lançamentos")]
    public async Task Deveria_Consolidar_Lancamentos_Corretamente()
    {
        await configuration.RunInScope(async (sp) =>
        {
            var dbContext = sp.GetRequiredService<CaixaDbContext>();
            var publishEndpoint = sp.GetRequiredService<IPublishEndpoint>();
            var testHarness = sp.GetRequiredService<ITestHarness>();

            var consolidacaoAnterior = new ConsolidacaoDiaria(
                new DateOnly(2025, 03, 31),
                50
            );
            consolidacaoAnterior.CalcularSaldoFinal();

            dbContext.Consolidacoes.Add(consolidacaoAnterior);
            dbContext.Lancamentos.AddRange(
                new LancamentoCaixa
                {
                    Descricao = "Entrada 1",
                    TipoLancamento = TipoLancamentoEnum.Entrada,
                    Valor = 500,
                    CriadoEm = new DateTime(2025, 04, 01, 11, 0, 0, DateTimeKind.Utc)
                }, new LancamentoCaixa
                {
                    Descricao = "Entrada 1",
                    TipoLancamento = TipoLancamentoEnum.Entrada,
                    Valor = 233.23m,
                    CriadoEm = new DateTime(2025, 04, 01, 11, 0, 0, DateTimeKind.Utc)
                }, new LancamentoCaixa
                {
                    Descricao = "Saida 1",
                    TipoLancamento = TipoLancamentoEnum.Saida,
                    Valor = -33.13m,
                    CriadoEm = new DateTime(2025, 04, 01, 12, 0, 0, DateTimeKind.Utc)
                },
                new LancamentoCaixa
                {
                    Descricao = "Saida 2",
                    TipoLancamento = TipoLancamentoEnum.Saida,
                    Valor = -105.10m,
                    CriadoEm = new DateTime(2025, 04, 01, 12, 12, 0, DateTimeKind.Utc)
                }
            );
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            await publishEndpoint.Publish(new LancamentoDiaAtualizadoEvent()
            {
                Data = new DateOnly(2025, 04, 01)
            });

            Assert.True(
                await testHarness.GetConsumerHarness<AtualizaConsolidacaoDiariaConsumer>()
                    .Consumed.Any<LancamentoDiaAtualizadoEvent>()
            );

            var consolidacao = await dbContext.Consolidacoes
                .Where(c => c.Data == new DateOnly(2025, 04, 01))
                .FirstOrDefaultAsync();

            Assert.NotNull(consolidacao);
            // Deve ser saldo do dia anterior
            Assert.Equal(50, consolidacao.SaldoInicial);
            // Deve ser total dos lançamentos de entrada
            Assert.Equal(733.23m, consolidacao.TotalEntradas);
            // Deve ser total dos lançamentos de saída
            Assert.Equal(-138.23m, consolidacao.TotalSaidas);
            // Deve ser total de saldo inicial, entradas e saidas
            Assert.Equal(645m, consolidacao.SaldoFinal);
        });
    }
}