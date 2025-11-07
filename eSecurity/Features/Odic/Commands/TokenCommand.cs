using eSecurity.Security.Authentication.Odic.Token;
using eSecurity.Security.Authentication.Odic.Token.Strategies;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Features.Odic.Commands;

public class TokenCommand() : IRequest<Result>
{
    public required string GrantType { get; set; }
    public required string ClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? RefreshToken { get; set; }
    public string? Code { get; set; }
    public string? ClientSecret { get; set; }
    public string? CodeVerifier { get; set; }
}

public class TokenCommandHandler(ITokenStrategyResolver tokenStrategyResolver) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver tokenStrategyResolver = tokenStrategyResolver;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        TokenPayload payload = request.GrantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeTokenPayload()
            {
                ClientId = request.ClientId,
                GrantType = request.GrantType,
                ClientSecret = request.ClientSecret,
                Code = request.Code,
                CodeVerifier = request.CodeVerifier,
                RedirectUri = request.RedirectUri
            },
            GrantTypes.RefreshToken => new RefreshTokenPayload()
            {
                ClientId = request.ClientId,
                GrantType = request.GrantType,
                ClientSecret = request.ClientSecret,
                RedirectUri = request.RedirectUri,
                RefreshToken = request.RefreshToken
            },
            _ => throw new NotSupportedException("Unsupported grant type")
        };
        
        var strategy = tokenStrategyResolver.Resolve(request.GrantType);
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}