using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public interface ITwoFactorService
{
    public ValueTask<ApiResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<ApiResponse> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<ApiResponse> PreferAsync(PreferTwoFactorMethodRequest request);
    
    public ValueTask<ApiResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    public ValueTask<ApiResponse> RegenerateQrCodeAsync(RegenerateQrCodeRequest request);
    
    public ValueTask<ApiResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<ApiResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
    
    public ValueTask<ApiResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<ApiResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request);
}