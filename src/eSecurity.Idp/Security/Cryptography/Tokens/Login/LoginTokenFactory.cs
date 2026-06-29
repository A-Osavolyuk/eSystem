using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authorization.Token;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Login;

public sealed class LoginTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    IClientQueryService clientQueryService,
    ISubjectProvider subjectProvider) : ITokenFactory<LoginTokenFactoryContext>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;

    public async ValueTask<TypedResult<string>> CreateAsync(
        LoginTokenFactoryContext context,
        TokenFactoryOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var subjectResult = await _subjectProvider.GetSubjectAsync(context.UserId, context.ClientId, cancellationToken);
        if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
        {
            return TypedResult<string>.Fail(new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var lifetime = context.TokenLifetime ?? _tokenConfigurations.DefaultLoginTokenLifetime;
        var tokenContext = new OpaqueTokenBuildContext
        {
            TokenLength = _tokenConfigurations.OpaqueTokenLength,
            TokenType = OpaqueTokenType.LoginToken,
            Sid = context.SessionId,
            ClientId = context.ClientId,
            ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
            Subject = subject,
        };
            
        var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}