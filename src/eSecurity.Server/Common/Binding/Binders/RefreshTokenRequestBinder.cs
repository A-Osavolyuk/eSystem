using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class RefreshTokenRequestBinder : IFormBinder<RefreshTokenRequest>
{
    public Task<TypedResult<RefreshTokenRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var result = TypedResult<RefreshTokenRequest>.Success(new RefreshTokenRequest()
        {
            ClientId = form["client_id"].ToString(),
            GrantType = form["grant_type"].ToString(),
            ClientSecret = form["client_secret"],
            RefreshToken = form["refresh_token"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
        });
        
        return Task.FromResult(result);
    }
}