using eCinema.Server.Security.Authentication.Oidc;
using eCinema.Server.Security.Authentication.Oidc.Constants;
using eCinema.Server.Security.Authentication.Oidc.Session;
using eCinema.Server.Security.Authentication.Oidc.Token;
using eCinema.Server.Security.Cookies;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using MediatR;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Features.Connect.Commands;

public record AuthorizeCallbackCommand : IRequest<Result>
{
    public required string? Code { get; init; }
    public required string? State { get; init; }
    public required string? Error { get; init; }
    public required string? ErrorDescription { get; init; }
}

public class AuthorizeCallbackCommandHandler(
    IOptions<ClientOptions> options,
    IConnectService connectService,
    ISessionProvider sessionProvider,
    ITokenProvider tokenProvider,
    ITokenValidator tokenValidator) : IRequestHandler<AuthorizeCallbackCommand, Result>
{
    private readonly IConnectService _connectService = connectService;
    private readonly ISessionProvider _sessionProvider = sessionProvider;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly ITokenValidator _tokenValidator = tokenValidator;
    private readonly ClientOptions _clientOptions = options.Value;

    public async Task<Result> Handle(AuthorizeCallbackCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Error) && !string.IsNullOrEmpty(request.ErrorDescription))
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri("https://localhost:6511/connect/error")
                .WithQueryParam("error", request.Error)
                .WithQueryParam("error_description", request.ErrorDescription)
                .Build());
        }
        
        var tokenRequest = new TokenRequest()
        {
            ClientId = _clientOptions.ClientId,
            GrantType = GrantTypes.AuthorizationCode,
            Code = request.Code,
            RedirectUri = _clientOptions.CallbackUri
        };
        
        var response = await _connectService.TokenAsync(tokenRequest, cancellationToken);
        if (!response.Succeeded)
        {
            var error = response.GetError();
            return Results.Found(QueryBuilder.Create()
                .WithUri("https://localhost:6511/connect/error")
                .WithQueryParam("error", error.Code)
                .WithQueryParam("error_description", error.Description)
                .Build());
        }

        if (!response.TryGetValue<TokenResponse>(out var tokenResponse) || tokenResponse is null)
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri("https://localhost:6511/connect/error")
                .WithQueryParam("error", ErrorTypes.OAuth.ServerError)
                .WithQueryParam("error_description", "Server error")
                .Build());
        }
        
        var accessTokenResult = await _tokenValidator.ValidateAsync(tokenResponse.AccessToken, cancellationToken);
        if (!accessTokenResult.Succeeded) return accessTokenResult;

        if (!string.IsNullOrEmpty(tokenResponse.IdToken))
        {
            var idTokenResult = await _tokenValidator.ValidateAsync(tokenResponse.IdToken, cancellationToken);
            if (!idTokenResult.Succeeded) return accessTokenResult;
        }

        var session = new SessionCookie()
        {
            Id = Guid.CreateVersion7(),
            IssuedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30),
        };
        
        _sessionProvider.Set(session);
        
        await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.AccessToken}", tokenResponse.AccessToken, 
            TimeSpan.FromMinutes(5), cancellationToken);

        if (!string.IsNullOrEmpty(tokenResponse.IdToken))
        {
            await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.IdToken}", tokenResponse.IdToken, 
                TimeSpan.FromMinutes(5), cancellationToken);
        }

        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.RefreshToken}", tokenResponse.RefreshToken, 
                TimeSpan.FromDays(30), cancellationToken);
        }

        return Results.Found("https://localhost:6511/app");
    }
}