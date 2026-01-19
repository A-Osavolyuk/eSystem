using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Consent;

public interface IConsentService
{
    public ValueTask<ApiResponse> CheckAsync(CheckConsentRequest request);
    public ValueTask<ApiResponse> GrantAsync(GrantConsentRequest request);
}