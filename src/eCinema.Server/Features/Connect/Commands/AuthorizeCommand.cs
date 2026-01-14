using eCinema.Server.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Authorization;
using eSystem.Core.Security.Authentication.Oidc.Client;
using MediatR;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Features.Connect.Commands;

public record AuthorizeCommand : IRequest<Result>;

public class AuthorizeCommandHandler(
    IOpenIdDiscoveryProvider discoveryProvider,
    IOptions<ClientOptions> options) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IOpenIdDiscoveryProvider _discoveryProvider = discoveryProvider;
    private readonly ClientOptions _clientOptions = options.Value;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var openIdConfiguration = await _discoveryProvider.GetOpenIdConfigurationsAsync();
        
        return Results.Found(QueryBuilder.Create()
            .WithUri(openIdConfiguration.AuthorizationEndpoint)
            .WithQueryParam("response_type", ResponseTypes.Code)
            .WithQueryParam("client_id", _clientOptions.ClientId)
            .WithQueryParam("redirect_uri", _clientOptions.PostLogoutRedirectUri)
            .WithQueryParam("state", Guid.NewGuid().ToString())
            .WithQueryParam("nonce", Guid.NewGuid().ToString())
            .WithQueryParam("scope", string.Join(" ", _clientOptions.SupportedScopes))
            .WithQueryParam("prompt", string.Join(" ", _clientOptions.SupportedPrompts))
            .Build());
    }
}