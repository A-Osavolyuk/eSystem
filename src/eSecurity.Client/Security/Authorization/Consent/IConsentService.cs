using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authorization.Consent;

public interface IConsentService
{
    public ValueTask<ApiResponse> CheckAsync(CheckConsentRequest request);
    public ValueTask<ApiResponse> GrantAsync(GrantConsentRequest request);
}