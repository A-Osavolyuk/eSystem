using eSystem.AppHost.Options;

namespace eSystem.AppHost.Extensions;

public static class DistributedApplicationExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<SqlServerServerResource> AddSqlServer()
        {
            var options = builder.Configuration.GetSectionValue<MsSqlOptions>("Configuration:MSSQL");
            var password = builder.AddParameter("mssql-password", options.Password);

            return builder.AddSqlServer(options.Name, password, options.Port);
        }

        public IResourceBuilder<RedisResource> AddRedis()
        {
            var options = builder.Configuration.GetSectionValue<RedisOptions>("Configuration:Redis");

            return builder.AddRedis(options.Name, options.Port);
        }

        public IResourceBuilder<RabbitMQServerResource> AddRabbitMq()
        {
            var options = builder.Configuration.GetSectionValue<RabbitMqOptions>("Configuration:RabbitMQ");
            var defaultPassword = builder.AddParameter("rabbit-mq-password", options.Password);
            var defaultUser = builder.AddParameter("rabbit-mq-user", options.User);

            return builder.AddRabbitMQ(options.Name, defaultUser, defaultPassword, options.Port);
        }
    }
}