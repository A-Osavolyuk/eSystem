using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.Consent;

public interface IConsentService
{
    public ValueTask<ApiResponse> CheckAsync(CheckConsentRequest request);
    public ValueTask<ApiResponse> GrantAsync(GrantConsentRequest request);
}