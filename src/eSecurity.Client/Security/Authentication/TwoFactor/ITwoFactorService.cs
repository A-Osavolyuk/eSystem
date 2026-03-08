using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public interface ITwoFactorService
{
    public ValueTask<ApiResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<ApiResponse> DisableAsync();
    public ValueTask<ApiResponse> PreferAsync(PreferTwoFactorMethodRequest request);
    
    public ValueTask<ApiResponse> GenerateQrCodeAsync();
    public ValueTask<ApiResponse> RegenerateQrCodeAsync();
    
    public ValueTask<ApiResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<ApiResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
    
    public ValueTask<ApiResponse> GenerateRecoveryCodesAsync();
    public ValueTask<ApiResponse> LoadRecoveryCodesAsync();
}