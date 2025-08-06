using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string FallbackUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    ISignInManager signInManager,
    IOAuthProviderManager providerManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly ISignInManager signInManager = signInManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var providers = await providerManager.GetAllAsync(cancellationToken);
        var isValidProvider = providers.Any(x => x.Name == request.Provider);

        if (!isValidProvider)
        {
            return Results.BadRequest($"Invalid oauth provider {request.Provider}.");
        }

        var redirectUri = UrlGenerator.Action("handle", "OAuth", new { request.ReturnUri });
        var fallbackUri = UrlGenerator.Url(request.FallbackUri, new { sessionId = string.Empty });
        
        var items = new Dictionary<string, string?>()
        {
            { "fallbackUri", fallbackUri }
        };
        
        var properties = signInManager.ConfigureAuthenticationProperties(redirectUri, items);
        
        var result = Result.Success(new OAuthLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return result;
    }
}