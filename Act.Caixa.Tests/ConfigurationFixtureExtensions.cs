using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Act.Caixa.Tests;

public static class ConfigurationFixtureExtensions
{
    public static async Task RunInScope(this ConfigurationFixture fixture, Func<IServiceProvider, Task> action)
    {
        using var scope = fixture.ServiceProvider.CreateScope();
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        
        await harness.Start();
        await action(scope.ServiceProvider);
        await harness.Stop();
    }
}