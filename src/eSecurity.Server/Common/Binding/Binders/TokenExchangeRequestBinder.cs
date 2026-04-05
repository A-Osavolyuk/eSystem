using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class TokenExchangeRequestBinder : IFormBinder<TokenExchangeRequest>
{
    public Task<TypedResult<TokenExchangeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (grantType is null)
        {
            return Task.FromResult(TypedResult<TokenExchangeRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var result = TypedResult<TokenExchangeRequest>.Success(new TokenExchangeRequest()
        {
            GrantType = grantType.Value,
            ClientId = form["client_id"].ToString(),
            ClientSecret = form["client_secret"],
            Scope = form["scope"],
            ActorToken = form["actor_token"],
            Audience = form["audience"],
            SubjectToken = form["subject_token"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
            ActorTokenType = EnumHelper.FromString<TokenType>(form["actor_token_type"])?.Value,
            RequestTokenType = EnumHelper.FromString<TokenType>(form["request_token_type"])?.Value,
            SubjectTokenType = EnumHelper.FromString<TokenType>(form["subject_token_type"])?.Value,
        });
        
        return Task.FromResult(result);
    }
}