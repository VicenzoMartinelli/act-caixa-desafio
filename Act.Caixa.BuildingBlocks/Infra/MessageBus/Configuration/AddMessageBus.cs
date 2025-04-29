using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Act.Caixa.BuildingBlocks.Infra.MessageBus.Configuration;

public static class MessageBusConfiguration
{
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMassTransit(bus =>
        {   
            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["rabbitmq_host"], "/", h =>
                {
                    h.Username(configuration["rabbitmq_user"]!);
                    h.Password(configuration["rabbitmq_user"]!);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
    
    public static IServiceCollection AddMessageBus<TBaseType>(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMassTransit(bus =>
        {   
            bus.AddConsumersFromNamespaceContaining<TBaseType>();
            
            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["rabbitmq_host"], "/", h =>
                {
                    h.Username(configuration["rabbitmq_user"]!);
                    h.Password(configuration["rabbitmq_user"]!);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
