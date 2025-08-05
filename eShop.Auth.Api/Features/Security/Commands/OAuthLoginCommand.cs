using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record OAuthLoginCommand(string Provider, string? ReturnUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    ISignInManager signInManager) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly ISignInManager signInManager = signInManager;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var providers = await signInManager.GetOAuthSchemasAsync(cancellationToken);
        var validProvider = providers.Any(x => x == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid oauth provider {request.Provider}.");
        }

        var redirectUri = UrlGenerator.Action("handle", "OAuth",
            new { ReturnUri = request.ReturnUri ?? "/" });
        
        var properties = signInManager.ConfigureOAuthProperties(request.Provider, redirectUri );;
        
        var result = Result.Success(new OAuthLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return result;
    }
}