using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.Ciba;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class CibaRequestBinder : IFormBinder<CibaRequest>
{
    public Task<TypedResult<CibaRequest>> BindAsync(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var result = TypedResult<CibaRequest>.Success(new CibaRequest()
        {
            GrantType = form["grant_type"].ToString(),
            ClientId = form["client_id"].ToString(),
            AuthReqId = form["auth_req_id"].ToString(),
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
            ClientSecret = form["client_secret"],
        });
        
        return Task.FromResult(result);
    }
}