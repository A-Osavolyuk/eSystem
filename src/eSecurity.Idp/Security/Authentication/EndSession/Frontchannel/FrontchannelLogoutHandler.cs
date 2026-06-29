using System.Net;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.EndSession.Frontchannel;

public sealed class FrontchannelLogoutHandler(
    IClientQueryService clientQueryService,
    IOptions<EndSessionOptions> options) : IFrontChannelLogoutHandler
{
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly EndSessionOptions _options = options.Value;

    public async ValueTask<Result> HandleAsync(EndSessionRequestEntity request, 
        CancellationToken cancellationToken = default)
    {
        var fallbackUri = _options.FallbackUrl;
        var loggedOutUri = _options.LoggedOutUrl;
        if (string.IsNullOrEmpty(fallbackUri) || string.IsNullOrEmpty(loggedOutUri))
            throw new InvalidOperationException("End session flow was not configured correctly");

        var errorUri = request.PostLogoutRedirectUri;
        if (string.IsNullOrEmpty(errorUri))
            errorUri = fallbackUri;
        
        var clients = await _clientQueryService.ListBySessionAsync(request.Session.Id, cancellationToken);
        if (clients.Count == 0)
            return Fallback(errorUri, ErrorCode.InvalidSession, "Session is invalid", request.State);

        var frontchannelLogoutUris = new List<string>();
        foreach (var client in clients)
        {
            var clientUris = await _clientQueryService.GetUrisAsync(client.Id, cancellationToken);
            var uris = clientUris
                .Where(x => x.Type == UriType.FrontChannelLogout)
                .Select(x => x.Uri)
                .ToList();
            
            frontchannelLogoutUris.AddRange(uris);
        }

        var iframesBuilder = new StringBuilder();
        foreach (var frontchannelLogoutUri in frontchannelLogoutUris)
            iframesBuilder.AppendLine($"</iframe src=\"{frontchannelLogoutUri}\" style=\"display:none\">");
        
        var redirectUri = request.PostLogoutRedirectUri;
        if (string.IsNullOrEmpty(redirectUri))
            redirectUri = loggedOutUri;

        var redirectDelay = _options.FrontchannelRedirectDelay?.TotalMilliseconds ?? 500;
        var frontchannelRedirectUriBuilder = QueryBuilder.Create()
            .WithUri(redirectUri);

        if (!string.IsNullOrEmpty(request.State))
            frontchannelRedirectUriBuilder.WithQueryParam("state", request.State);
        
        var html = 
            $$"""
                <!DOCTYPE html>
                <html>
                <head>
                  <meta charset="UTF-8">
                  <title>Logging out...</title>
                </head>
                <body>
                  {{iframesBuilder}}
                  <script>
                    setTimeout(function() {
                      window.location.replace('{{frontchannelRedirectUriBuilder.Build()}}');
                    }, {{redirectDelay}});
                  </script>
                </body>
                </html>
            """;

        return Results.Html(HttpStatusCode.OK, html);
    }
    
    private static Result Fallback(string uri, ErrorCode error, string description, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(uri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", description);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }
}