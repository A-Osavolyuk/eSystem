using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<HttpResponse<SignInResponse>> SignInAsync(SignInRequest request);
    public ValueTask<HttpResponse<SignUpResponse>> SignUpAsync(SignUpRequest request);
    
    public ValueTask<HttpResponse<SignInSessionDto>> LoadSignInSessionAsync(Guid sid);
    
    public ValueTask<HttpResponse<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request);
}