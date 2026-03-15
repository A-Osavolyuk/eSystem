using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Ciba;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class CibaRequestBinder : IFormBinder<CibaRequest>
{
    public Task<TypedResult<CibaRequest>> BindAsync(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (!grantType.HasValue)
        {
            return Task.FromResult(TypedResult<CibaRequest>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var result = TypedResult<CibaRequest>.Success(new CibaRequest()
        {
            GrantType = grantType.Value,
            ClientId = form["client_id"].ToString(),
            AuthReqId = form["auth_req_id"].ToString(),
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
            ClientSecret = form["client_secret"],
        });
        
        return Task.FromResult(result);
    }
}