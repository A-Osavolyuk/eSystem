using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class DeviceCodeRequestBinder : IFormBinder<DeviceCodeRequest>
{
    public Task<TypedResult<DeviceCodeRequest>> BindAsync(IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        var grantType = EnumHelper.FromString<GrantType>(form["grant_type"].ToString());
        if (grantType is null)
        {
            return Task.FromResult(TypedResult<DeviceCodeRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            }));
        }
        
        var assertionsTypeString = form["client_assertion_type"].ToString();
        var result = TypedResult<DeviceCodeRequest>.Success(new DeviceCodeRequest()
        {
            ClientId = form["client_id"].ToString(),
            GrantType = grantType.Value,
            DeviceCode = form["device_code"],
            ClientSecret = form["client_secret"],
            ClientAssertion = form["client_assertion"],
            ClientAssertionType = EnumHelper.FromString<AssertionType>(assertionsTypeString)?.Value
        });

        return Task.FromResult(result);
    }
}