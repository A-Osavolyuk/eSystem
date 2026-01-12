using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.Consent;

public interface IConsentService
{
    public ValueTask<HttpResponse<CheckConsentResponse>> CheckAsync(CheckConsentResponse request);
    public ValueTask<HttpResponse> GrantAsync(GrantConsentsRequest request);
}