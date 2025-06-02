using eShop.MessageBus.Builders;
using eShop.MessageBus.Bus;
using eShop.MessageBus.Configurations;
using eShop.MessageBus.Interfaces;

namespace eShop.MessageBus.Extensions;

public static class MessageBusExtensions
{
    public static IReadOnlyList<QueueConfiguration> AddMessageBus(this IHostApplicationBuilder builder, Action<IMessageBusBuilder> configure)
    {
        var busBuilder = new MessageBusBuilder();
        configure(busBuilder);

        var config = busBuilder.Build();
        
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<MessageBusRegistry>();

        return config;
    }
}