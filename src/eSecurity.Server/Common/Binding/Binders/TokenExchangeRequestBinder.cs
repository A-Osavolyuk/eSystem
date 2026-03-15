using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class TokenExchangeRequestBinder : IFormBinder<TokenExchangeRequest>
{
    public Task<TypedResult<TokenExchangeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (!grantType.HasValue)
        {
            return Task.FromResult(TypedResult<TokenExchangeRequest>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
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
            ActorTokenType = form["actor_token_type"],
            Audience = form["audience"],
            RequestTokenType = form["request_token_type"],
            SubjectToken = form["subject_token"],
            SubjectTokenType = form["subject_token_type"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
        });
        
        return Task.FromResult(result);
    }
}