using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class ClientCredentialsRequestBinder : IFormBinder<ClientCredentialsRequest>
{
    public Task<TypedResult<ClientCredentialsRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (grantType is null)
        {
            return Task.FromResult(TypedResult<ClientCredentialsRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var assertionsTypeString = form["client_assertion_type"].ToString();
        var result = TypedResult<ClientCredentialsRequest>.Success(new ClientCredentialsRequest()
        {
            ClientId = form["client_id"].ToString(),
            GrantType = grantType.Value,
            ClientSecret = form["client_secret"],
            Scope = form["scope"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = EnumHelper.FromString<AssertionType>(assertionsTypeString)?.Value,
        });
        
        return Task.FromResult(result);
    }
}