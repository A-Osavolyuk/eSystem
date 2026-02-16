using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Server.Features.Connect.Commands;

public record TokenCommand(Dictionary<string, string> Request) : IRequest<Result>;

public class TokenCommandHandler(
    ITokenStrategyResolver tokenStrategyResolver,
    ITokenRequestMapper tokenRequestMapper,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver _tokenStrategyResolver = tokenStrategyResolver;
    private readonly ITokenRequestMapper _tokenRequestMapper = tokenRequestMapper;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        if (!request.Request.TryGetValue("grant_type", out var grantType) || string.IsNullOrEmpty(grantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "grant_type is required"
            });
        }
        
        if (!_configuration.GrantTypesSupported.Contains(grantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = $"'{grantType}' grant type is not supported"
            });
        }

        if (!request.Request.TryGetValue("client_id", out var clientId) || string.IsNullOrEmpty(clientId))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var tokenRequest = _tokenRequestMapper.Map(request.Request);
        if (tokenRequest is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{grantType}' grant type is allowed, but not supported"
            });
        }

        var strategy = _tokenStrategyResolver.Resolve(grantType);
        return await strategy.ExecuteAsync(tokenRequest, cancellationToken);
    }
}