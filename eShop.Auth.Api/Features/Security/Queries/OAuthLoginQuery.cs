using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record OAuthLoginQuery(string Provider, string? ReturnUri) : IRequest<Result>;

internal sealed class OAuthLoginQueryHandler(
    ISignInManager signInManager) : IRequestHandler<OAuthLoginQuery, Result>
{
    private readonly ISignInManager signInManager = signInManager;

    public async Task<Result> Handle(OAuthLoginQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await signInManager.GetOAuthSchemasAsync(cancellationToken);
        var validProvider = providers.Any(x => x == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid oauth provider {request.Provider}.");
        }

        var redirectUri = UrlGenerator.Action("handle-oauth-login", "OAuth",
            new { ReturnUri = request.ReturnUri ?? "/" });
        
        var properties = signInManager.ConfigureOAuthProperties(request.Provider, redirectUri );;
        
        var result = Result.Success(new OAuthLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return await Task.FromResult(result);
    }
}