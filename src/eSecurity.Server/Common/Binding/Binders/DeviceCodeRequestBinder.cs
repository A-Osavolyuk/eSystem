using eSystem.Core.Binding;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class DeviceCodeRequestBinder : IFormBinder<DeviceCodeRequest>
{
    public Task<TypedResult<DeviceCodeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var result = TypedResult<DeviceCodeRequest>.Success(new DeviceCodeRequest()
        {
            ClientId = form["client_id"].ToString(),
            GrantType = form["grant_type"].ToString(),
            DeviceCode = form["device_code"],
            ClientSecret = form["client_secret"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = form["client_assertion_type"]
        });

        return Task.FromResult(result);
    }
}