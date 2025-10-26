using eSystem.AppHost.Options;
using Microsoft.Extensions.Configuration;

namespace eSystem.AppHost.Extensions;

public static class MongoExpressExtensions
{
    private static IResourceBuilder<MongoExpressContainerResource> WithAuthentication(
        this IResourceBuilder<MongoExpressContainerResource> builder, string name = "admin", string password = "admin")
    {
        builder.WithEnvironment("ME_CONFIG_BASICAUTH", "true");
        builder.WithEnvironment("ME_CONFIG_BASICAUTH_USERNAME", name);
        builder.WithEnvironment("ME_CONFIG_BASICAUTH_PASSWORD", password);
        
        return builder;
    }

    private static IResourceBuilder<MongoExpressContainerResource> WithMongoCredentials(
        this IResourceBuilder<MongoExpressContainerResource> builder, string name, string password)
    {
        builder.WithEnvironment("ME_CONFIG_MONGODB_ADMINUSERNAME", name); 
        builder.WithEnvironment("ME_CONFIG_MONGODB_ADMINPASSWORD", password);
        
        return builder;
    }

    private static IResourceBuilder<MongoExpressContainerResource> WithMongoServer(
        this IResourceBuilder<MongoExpressContainerResource> builder, string server)
    {
        builder.WithEnvironment("ME_CONFIG_MONGODB_SERVER", server);
        
        return builder;
    }

    private static IResourceBuilder<MongoExpressContainerResource> WithMongoUrl(
        this IResourceBuilder<MongoExpressContainerResource> builder, string url)
    {
        builder.WithEnvironment("ME_CONFIG_MONGODB_URL", url);
        
        return builder;
    }

    public static IResourceBuilder<MongoDBServerResource> WithMongoExpress(
        this IResourceBuilder<MongoDBServerResource> builder, IConfigurationManager configuration)
    {
        var options = configuration.GetSectionValue<MongoExpressOptions>("Configuration:MongoExpress");
        
        return builder.WithMongoExpress(cfg =>
        {
            cfg.WithAuthentication();
            cfg.WithMongoCredentials(options.User, options.Password);
            cfg.WithMongoServer(options.MongoServer);
            cfg.WithMongoUrl(options.MongoUrl);
        }, options.Name);
    }
}