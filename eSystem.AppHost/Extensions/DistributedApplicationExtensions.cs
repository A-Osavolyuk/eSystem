using eSystem.AppHost.Options;

namespace eSystem.AppHost.Extensions;

public static class DistributedApplicationExtensions
{
    public static IResourceBuilder<SqlServerServerResource> AddSqlServer(this IDistributedApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSectionValue<MsSqlOptions>("Configuration:MSSQL");
        var password = builder.AddParameter("mssql-password", options.Password);

        return builder.AddSqlServer(options.Name, password, options.Port);
    }

    public static IResourceBuilder<RedisResource> AddRedis(this IDistributedApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSectionValue<RedisOptions>("Configuration:Redis");

        return builder.AddRedis(options.Name, options.Port);
    }

    public static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(this IDistributedApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSectionValue<RabbitMqOptions>("Configuration:RabbitMQ");
        var defaultPassword = builder.AddParameter("rabbit-mq-password", options.Password);
        var defaultUser = builder.AddParameter("rabbit-mq-user", options.User);

        return builder.AddRabbitMQ(options.Name, defaultUser, defaultPassword, options.Port);
    }
}