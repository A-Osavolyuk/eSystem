using eSecurity.Server.Common.Responses;
using eSecurity.Server.Security.Identity.Options;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Features.OAuth;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string State) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(IOptions<SignInOptions> options) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly SignInOptions _options = options.Value;

    public Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (!_options.AllowOAuthLogin)
        {
            return Task.FromResult(Results.InternalServerError(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", ErrorTypes.OAuth.ServerError)
                .WithQueryParam("error_description", "OAuth is not allowed")
                .Build()));
        }
        
        var builder = QueryBuilder.Create()
            .WithUri("/api/v1/oauth/handle")
            .WithQueryParam("returnUri", request.ReturnUri);
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = builder.Build(),
            Items = 
            {
                { "state", request.State },
            }
        };
        
        var result = Results.Ok(new OAuthLoginResponse
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return Task.FromResult(result);
    }
}