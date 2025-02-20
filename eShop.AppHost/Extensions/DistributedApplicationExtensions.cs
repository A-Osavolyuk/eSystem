using eShop.AppHost.Options;
using Microsoft.Extensions.Configuration;

namespace eShop.AppHost.Extensions;

public static class DistributedApplicationExtensions
{
    public static IResourceBuilder<ProjectResource> WaitForReference(this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<IResourceWithConnectionString> resource)
    {
        builder.WithReference(resource);
        builder.WaitFor(resource);

        return builder;
    }

    public static IResourceBuilder<ProjectResource> WaitForReference(this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<ProjectResource> resource)
    {
        builder.WithReference(resource);
        builder.WaitFor(resource);

        return builder;
    }

    public static IResourceBuilder<ExecutableResource> WaitForReference(
        this IResourceBuilder<ExecutableResource> builder, IResourceBuilder<ProjectResource> resource)
    {
        builder.WithReference(resource);
        builder.WaitFor(resource);

        return builder;
    }
    
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
    
    public static IResourceBuilder<MongoDBServerResource> AddMongoDb(this IDistributedApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSectionValue<MongoDbOptions>("Configuration:MongoDB");
        var password = builder.AddParameter("mongo-password", options.Password);
        var user = builder.AddParameter("mongo-user", options.User);

        return builder.AddMongoDB(options.Name, options.Port, user, password);
    }
    
    public static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(this IDistributedApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSectionValue<RabbitMqOptions>("Configuration:RabbitMQ");
        var defaultPassword = builder.AddParameter("rabbit-mq-password", options.Password);
        var defaultUser = builder.AddParameter("rabbit-mq-user", options.User);

        return builder.AddRabbitMQ(options.Name, defaultUser, defaultPassword, options.Port);
    }
}