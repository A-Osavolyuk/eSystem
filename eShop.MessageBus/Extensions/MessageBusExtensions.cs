using eShop.MessageBus.Builders;
using eShop.MessageBus.Bus;
using eShop.MessageBus.Configurations;

namespace eShop.MessageBus.Extensions;

public static class MessageBusExtensions
{
    public static IReadOnlyList<QueueConfiguration> AddMessageBus(this IHostApplicationBuilder builder, Action<MessageBusBuilder> configure)
    {
        var busBuilder = new MessageBusBuilder();
        configure(busBuilder);

        var queues = busBuilder.Build();
        builder.Services.AddSingleton(queues);
        builder.Services.AddSingleton<MessageBusRegistry>();

        return queues;
    }
}