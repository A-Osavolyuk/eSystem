using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class RefreshTokenRequestBinder : IFormBinder<RefreshTokenRequest>
{
    public Task<TypedResult<RefreshTokenRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (grantType is null)
        {
            return Task.FromResult(TypedResult<RefreshTokenRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var result = TypedResult<RefreshTokenRequest>.Success(new RefreshTokenRequest()
        {
            GrantType = grantType.Value,
            ClientId = form["client_id"].ToString(),
            ClientSecret = form["client_secret"],
            RefreshToken = form["refresh_token"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
        });
        
        return Task.FromResult(result);
    }
}