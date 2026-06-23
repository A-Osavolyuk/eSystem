using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Binding;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Idp.Common.Binding.Binders;

public sealed class TokenExchangeRequestBinder : IFormBinder<TokenExchangeRequest>
{
    public Task<TypedResult<TokenExchangeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.ParseFromString<GrantType>(form["grant_type"].ToString());
        if (grantType is null)
        {
            return Task.FromResult(TypedResult<TokenExchangeRequest>.Fail(new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var assertionsTypeString = form["client_assertion_type"].ToString();
        var result = TypedResult<TokenExchangeRequest>.Success(new TokenExchangeRequest
        {
            GrantType = grantType.Value,
            ClientId = form["client_id"].ToString(),
            ClientSecret = form["client_secret"],
            Scope = form["scope"],
            ActorToken = form["actor_token"],
            Audience = form["audience"],
            SubjectToken = form["subject_token"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = EnumHelper.ParseFromString<AssertionType>(assertionsTypeString)?.Value,
            ActorTokenType = EnumHelper.ParseFromString<TokenType>(form["actor_token_type"].ToString())?.Value,
            RequestTokenType = EnumHelper.ParseFromString<TokenType>(form["request_token_type"].ToString())?.Value,
            SubjectTokenType = EnumHelper.ParseFromString<TokenType>(form["subject_token_type"].ToString())?.Value,
        });
        
        return Task.FromResult(result);
    }
}