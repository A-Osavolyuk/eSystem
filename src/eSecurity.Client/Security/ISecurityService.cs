using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<ApiResponse> SignInAsync(SignInRequest request);
    public ValueTask<ApiResponse> SignUpAsync(SignUpRequest request);
    
    public ValueTask<ApiResponse> LoadSignInSessionAsync(Guid sid);
    
    public ValueTask<ApiResponse> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<ApiResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<ApiResponse> UnlockAccountAsync(UnlockAccountRequest request);
}