using eSystem.Core.Primitives;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authorization.Authorize;

public sealed class RedirectManager(IOptions<AuthorizationOptions> options)
{
    private readonly AuthorizationOptions _options = options.Value;

    public string GetRedirectUri(ErrorCode error, string description, string? state = null)
    {
        if (string.IsNullOrEmpty(_options.FallbackUrl))
            throw new Exception("Authorization options was not configured");
        
        return BuildUri(_options.FallbackUrl, error, description, state);
    }

    public string GetRedirectUri(string uri, ErrorCode error, string description, string? state = null)
    {
        return BuildUri(uri, error, description, state);
    }

    private string BuildUri(string uri, ErrorCode error, string description, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(uri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", description);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return builder.Build();
    }
}