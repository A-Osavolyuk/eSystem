using System.Text.Json;
using eSystem.AppHost.Options;

namespace eSystem.AppHost.Extensions;

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
            .WithEnvironment("Jwt__AccessTokenExpirationMinutes", options.AccessTokenExpirationMinutes.ToString());
    }
}