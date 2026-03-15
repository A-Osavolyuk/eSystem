using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Features.Connect.Commands;

public record TokenCommand(IFormCollection Form) : IRequest<Result>;

public class TokenCommandHandler(
    ITokenStrategyResolver tokenStrategyResolver,
    IFormBindingProvider bindingProvider,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<TokenCommand, Result>
{
    private readonly ITokenStrategyResolver _tokenStrategyResolver = tokenStrategyResolver;
    private readonly IFormBindingProvider _bindingProvider = bindingProvider;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public async Task<Result> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        if (!request.Form.TryGetValue("grant_type", out var grantTypeString) || string.IsNullOrEmpty(grantTypeString))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "grant_type is required"
            });
        }
        
        var grantType = EnumHelper.FromString<GrantType>(grantTypeString.ToString());
        if (!grantType.HasValue)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "grant_type is invalid."
            });
        }
        
        if (!_configuration.GrantTypesSupported.Contains(grantType.Value))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = $"'{grantType}' grant type is not supported"
            });
        }

        if (!request.Form.TryGetValue("client_id", out var clientId) || string.IsNullOrEmpty(clientId))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var binder = _bindingProvider.GetRequiredBinder<TokenRequest>();
        var tokenResult = await binder.BindAsync(request.Form, cancellationToken);
        if (!tokenResult.Succeeded || !tokenResult.TryGetValue(out var tokenRequest))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{grantType}' grant type is allowed, but not supported"
            });
        }

        var strategy = _tokenStrategyResolver.Resolve(grantType.Value);
        return await strategy.ExecuteAsync(tokenRequest, cancellationToken);
    }
}