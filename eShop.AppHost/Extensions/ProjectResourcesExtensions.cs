using eShop.AppHost.Options;

namespace eShop.AppHost.Extensions;

public static class ProjectResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> WithJwtConfig(this IResourceBuilder<ProjectResource> builder)
    {
        var configuration = builder.ApplicationBuilder.Configuration;
        var options = configuration.GetSectionValue<JwtOptions>("Configuration:Jwt");
        
        return builder
            .WithEnvironment("Jwt__Secret", options.Secret)
            .WithEnvironment("Jwt__Issuer", options.Issuer)
            .WithEnvironment("Jwt__Audience", options.Audience)
            .WithEnvironment("Jwt__ExpirationDays", options.ExpirationDays.ToString());
    }

    public static IResourceBuilder<ProjectResource> AddProject<TProject>(this IDistributedApplicationBuilder builder,
        string name, bool excludeLaunchProperties) where TProject : IProjectMetadata, new()
    {
        return builder.AddProject<TProject>(name, cfg =>
        {
            cfg.ExcludeLaunchProfile = excludeLaunchProperties;
        });
    }
}