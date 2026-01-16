using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<ApiResponse<SignInResponse>> SignInAsync(SignInRequest request);
    public ValueTask<ApiResponse<SignUpResponse>> SignUpAsync(SignUpRequest request);
    
    public ValueTask<ApiResponse<SignInSessionDto>> LoadSignInSessionAsync(Guid sid);
    
    public ValueTask<ApiResponse<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request);
}