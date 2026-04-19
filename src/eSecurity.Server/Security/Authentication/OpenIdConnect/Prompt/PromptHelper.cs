using eSystem.Core.Primitives;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt;

public static class PromptHelper
{
    public static string GetRedirectUri(string redirectUri, ErrorCode error, 
        string errorDescription, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(redirectUri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", errorDescription);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return builder.Build();
    }
}