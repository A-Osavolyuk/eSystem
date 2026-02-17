using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<ApiResponse> SignInAsync(SignInRequest request);
    public ValueTask<ApiResponse> SignUpAsync(SignUpRequest request);
    public ValueTask<ApiResponse> CompleteSignUpAsync(CompleteSignUpRequest request);
    public ValueTask<ApiResponse> GetAuthenticationSessionAsync(Guid sid);
    
    public ValueTask<ApiResponse> CheckAccountAsync(CheckAccountRequest request);
}