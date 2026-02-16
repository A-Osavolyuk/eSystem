using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

public class LogoutHandler(
    IOptions<OpenIdConfiguration> configuration,
    IClientManager clientManager,
    ISessionManager sessionManager,
    ILogoutStrategyResolver logoutStrategyResolver) : ILogoutHandler
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ILogoutStrategyResolver _logoutStrategyResolver = logoutStrategyResolver;
    private readonly OpenIdConfiguration _configuration = configuration.Value;

    public async ValueTask<Result> HandleAsync(LogoutContext context, CancellationToken cancellationToken = default)
    {
        var session = await _sessionManager.FindByIdAsync(Guid.Parse(context.Sid), cancellationToken);
        if (session is null)
        {
            return Results.InternalServerError(new Error
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Invalid session."
            });
        }

        var client = await _clientManager.FindByIdAsync(context.Audience, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });
        }
        
        var postLogoutRedirectUri = client.Uris.FirstOrDefault(
            x => x.Type == UriType.PostLogoutRedirect && x.Uri == context.PostLogoutRedirectUri);
        
        if (postLogoutRedirectUri is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "post_logout_redirect_uri is invalid."
            });
        }
        
        var response = new LogoutResponse()
        {
            State = context.State,
            PostLogoutRedirectUri = postLogoutRedirectUri.Uri
        };

        if (_configuration.FrontchannelLogoutSupported)
        {
            var strategy = _logoutStrategyResolver.Resolve<List<string>>(LogoutFlow.Frontchannel);
            response.FrontChannelLogoutUris = await strategy.ExecuteAsync(session, cancellationToken);
        }

        if (_configuration.BackchannelLogoutSupported)
        {
            var strategy = _logoutStrategyResolver.Resolve<Result>(LogoutFlow.Backchannel);
            var result = await strategy.ExecuteAsync(session, cancellationToken);
            if (!result.Succeeded) return result;
        }
        
        return Results.Ok(response);
    }
}