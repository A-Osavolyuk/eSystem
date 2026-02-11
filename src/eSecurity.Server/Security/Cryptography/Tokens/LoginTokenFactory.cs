using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class LoginTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider) : ITokenFactory
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;

    public async ValueTask<TokenResult> CreateAsync(
        ClientEntity client, 
        UserEntity? user = null, 
        SessionEntity? session = null,
        TokenFactoryOptions? factoryOptions = null, 
        CancellationToken cancellationToken = default)
    {
        if (user is null)
        {
            return TokenResult.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
        var lifetime = client.LoginTokenLifetime ?? _tokenConfigurations.DefaultLoginTokenLifetime;
        var tokenContext = new OpaqueTokenBuildContext
        {
            TokenLength = _tokenConfigurations.OpaqueTokenLength,
            TokenType = OpaqueTokenType.LoginToken,
            Sid = session?.Id,
            ClientId = client.Id,
            ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
            Subject = user.Id.ToString(),
        };
            
        var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TokenResult.Success(token);
    }
}