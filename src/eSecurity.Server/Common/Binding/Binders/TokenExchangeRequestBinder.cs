using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class TokenExchangeRequestBinder : IFormBinder<TokenExchangeRequest>
{
    public Task<TypedResult<TokenExchangeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var result = TypedResult<TokenExchangeRequest>.Success(new TokenExchangeRequest()
        {
            GrantType = form["grant_type"].ToString(),
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