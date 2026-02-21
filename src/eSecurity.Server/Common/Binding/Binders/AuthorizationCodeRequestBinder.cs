using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class AuthorizationCodeRequestBinder : IFormBinder<AuthorizationCodeRequest>
{
    public Task<TypedResult<AuthorizationCodeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var result = TypedResult<AuthorizationCodeRequest>.Success(new AuthorizationCodeRequest()
        {
            ClientId = form["client_id"].ToString(),
            ClientSecret = form["client_secret"].ToString(),
            GrantType = form["grant_type"].ToString(),
            Code = form["code"].ToString(),
            CodeVerifier = form["code_verifier"].ToString(),
            RedirectUri = form["redirect_uri"].ToString(),
            ClientAssertion = form["client_assertion"].ToString(),
            ClientAssertionType = form["client_assertion_type"].ToString(),
        });
        
        return Task.FromResult(result);
    }
}