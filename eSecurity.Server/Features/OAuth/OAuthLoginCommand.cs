using eSecurity.Server.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Identity.Options;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Authentication;
using OtpNet;

namespace eSecurity.Server.Features.OAuth;

public sealed record OAuthLoginCommand(string Type, string ReturnUri, string FallbackUri) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    IOAuthSessionManager sessionManager,
    IOptions<SignInOptions> options) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly IOAuthSessionManager _sessionManager = sessionManager;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var fallbackUri = request.FallbackUri;
        if (!_options.AllowOAuthLogin)
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri(fallbackUri)
                .WithQueryParam("error", Errors.OAuth.ServerError)
                .WithQueryParam("error_description", "OAuth is not allowed")
                .Build());
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
        
        var builder = QueryBuilder.Create()
            .WithUri("/api/v1/oauth/handle")
            .WithQueryParam("returnUri", request.ReturnUri);
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = builder.Build(),
            Items = 
            {
                { "fallbackUri", fallbackUri },
                { "sessionId", session.Id.ToString() },
                { "token", token },
            }
        };
        
        await _sessionManager.CreateAsync(session, cancellationToken);
        
        var result = Results.Ok(new OAuthLoginResponse()
        {
            Provider = request.Type,
            AuthenticationProperties = properties
        });
        
        return result;
    }
}