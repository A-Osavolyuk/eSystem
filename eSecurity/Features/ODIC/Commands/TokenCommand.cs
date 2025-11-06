using eSecurity.Security.Authentication.Odic.Token;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.ODIC.Commands;

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
        var strategy = tokenStrategyResolver.Resolve(request.GrantType);
        return await strategy.HandleAsync(request, cancellationToken);
    }
}