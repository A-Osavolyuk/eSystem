using eShop.Auth.Api.Security.Identity.Options;
using eShop.Domain.Common.Results;
using eShop.Domain.Responses.Auth;
using Microsoft.AspNetCore.Authentication;
using OtpNet;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record LinkedAccountLoginCommand(string Type, string ReturnUri, string FallbackUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    IOAuthSessionManager sessionManager,
    IOptions<SignInOptions> options) : IRequestHandler<LinkedAccountLoginCommand, Result>
{
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly SignInOptions options = options.Value;

    public async Task<Result> Handle(LinkedAccountLoginCommand request,
        CancellationToken cancellationToken)
    {
        var fallbackUri = request.FallbackUri;
        
        if (!options.AllowOAuthLogin)
        {
            return Results.BadRequest("Signing with linked account is not allowed.", fallbackUri);
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
        
        await sessionManager.CreateAsync(session, cancellationToken);
        
        var result = Result.Success(new OAuthLoginResponse()
        {
            Provider = request.Type,
            AuthenticationProperties = properties
        });
        
        return result;
    }
}