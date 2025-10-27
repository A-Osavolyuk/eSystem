using System.Text.Json;
using eSystem.AppHost.Options;

namespace eSystem.AppHost.Extensions;

public static class ProjectResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> WithJwtConfig(this IResourceBuilder<ProjectResource> builder)
    {
        var configuration = builder.ApplicationBuilder.Configuration;
        var options = configuration.GetSectionValue<JwtOptions>("Configuration:Jwt");
        var audiencesJson = JsonSerializer.Serialize(options.Audiences);
        
        return builder
            .WithEnvironment("Jwt__Secret", options.Secret)
            .WithEnvironment("Jwt__Issuer", options.Issuer)
            .WithEnvironment("Jwt__Audiences", audiencesJson)
            .WithEnvironment("Jwt__AccessTokenExpirationMinutes", options.AccessTokenExpirationMinutes.ToString())
            .WithEnvironment("Jwt__RefreshTokenExpirationDays", options.RefreshTokenExpirationDays.ToString());
    }
}