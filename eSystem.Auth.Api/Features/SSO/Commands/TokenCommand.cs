using eSystem.Auth.Api.Security.Authentication.SSO.Token;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record TokenCommand(TokenRequest Request) : IRequest<Result>;

public class TokenCommandHandler(ITokenStrategyResolver tokenStrategyResolver) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver tokenStrategyResolver = tokenStrategyResolver;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        var strategy = tokenStrategyResolver.Resolve(request.Request.GrantType);
        return await strategy.HandleAsync(request.Request, cancellationToken);
    }
}