using eSecurity.Server.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Identity.Options;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Features.OAuth.Commands;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string State) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    IOptions<SignInOptions> options,
    IAuthenticationSessionManager authenticationSessionManager) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (!_options.AllowOAuthLogin)
        {
            return Results.InternalServerError(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", ErrorTypes.OAuth.ServerError)
                .WithQueryParam("error_description", "OAuth is not allowed")
                .Build());
        }
        
        var builder = QueryBuilder.Create()
            .WithUri("/api/v1/oauth/handle")
            .WithQueryParam("returnUri", request.ReturnUri);

        var session = new AuthenticationSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            IdentityProvider = request.Provider,
            PassedAuthenticationMethods = [AuthenticationMethods.Federated, AuthenticationMethods.Social],
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        
        var result = await _authenticationSessionManager.CreateAsync(session, cancellationToken);
        if (!result.Succeeded) return result;
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = builder.Build(),
            Items = 
            {
                { "state", request.State },
                { "sid", session.Id.ToString() }
            }
        };
        
        return Results.Ok(new OAuthLoginResponse
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
    }
}