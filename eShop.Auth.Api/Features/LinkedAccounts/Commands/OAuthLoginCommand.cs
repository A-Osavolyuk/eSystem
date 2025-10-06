using eShop.Domain.Responses.Auth;
using Microsoft.AspNetCore.Authentication;
using OtpNet;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string FallbackUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    IOAuthProviderManager providerManager,
    IOAuthSessionManager sessionManager,
    IdentityOptions identityOptions) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (!identityOptions.SignIn.AllowOAuthLogin)
        {
            return Failure(request.FallbackUri, request.Provider, OAuthErrorType.Unavailable);
        }
        
        var providers = await providerManager.GetAllAsync(cancellationToken);

        if (providers.Count == 0)
        {
            return Failure(request.FallbackUri, request.Provider, OAuthErrorType.InternalError);
        }
        
        var provider = providers.FirstOrDefault(x => x.Name == request.Provider);

        if (provider is null)
        {
            return Failure(request.FallbackUri, request.Provider, OAuthErrorType.UnsupportedProvider);
        }
        
        var randomBytes = KeyGeneration.GenerateRandomKey(20);
        var token = Base32Encoding.ToString(randomBytes);
        
        var session = new OAuthSessionEntity()
        {
            Id = Guid.NewGuid(),
            Token = token,
            CreateDate = DateTimeOffset.UtcNow,
            ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10),
        };
        
        var redirectUri = UrlGenerator.Action("handle", "OAuth", new { request.ReturnUri });
        var fallbackUri = request.FallbackUri;
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
            Items = 
            {
                { "fallbackUri", fallbackUri },
                { "sessionId", session.Id.ToString() },
                { "token", token },
            }
        };
        
        var result = Result.Success(new OAuthLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        session.ProviderId = provider.Id;
        await sessionManager.CreateAsync(session, cancellationToken);
        
        return result;
    }

    private Result Failure(string fallbackUrl, string provider, OAuthErrorType error)
    {
        var url = UrlGenerator.Url(fallbackUrl, new
        {
            ErrorCode = nameof(OAuthErrorType.UnsupportedProvider), 
            Provider = provider
        });
            
        return Results.Redirect(url);
    }
}