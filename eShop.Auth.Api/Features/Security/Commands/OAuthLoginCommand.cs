using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;
using OtpNet;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string FallbackUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    IOAuthProviderManager providerManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var providers = await providerManager.GetAllAsync(cancellationToken);

        if (providers.Count == 0)
        {
            var url = UrlGenerator.Url(request.FallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Provider = request.Provider
            });
            return Results.Redirect(url);
        }
        
        var provider = providers.FirstOrDefault(x => x.Name == request.Provider);

        if (provider is null)
        {
            var url = UrlGenerator.Url(request.FallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.UnsupportedProvider), 
                Provider = request.Provider
            });
            
            return Results.Redirect(url);
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
}