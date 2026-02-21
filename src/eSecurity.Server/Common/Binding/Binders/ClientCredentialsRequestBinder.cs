using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class ClientCredentialsRequestBinder : IFormBinder<ClientCredentialsRequest>
{
    public Task<TypedResult<ClientCredentialsRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var result = TypedResult<ClientCredentialsRequest>.Success(new ClientCredentialsRequest()
        {
            ClientId = form["client_id"].ToString(),
            GrantType = form["grant_type"].ToString(),
            ClientSecret = form["client_secret"],
            Scope = form["scope"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"],
        });
        
        return Task.FromResult(result);
    }
}