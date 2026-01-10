using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public interface ITwoFactorService
{
    public ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request);
    
    public ValueTask<HttpResponse<QrCode>> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    public ValueTask<HttpResponse<QrCode>> RegenerateQrCodeAsync(RegenerateQrCodeRequest request);
    
    public ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
    
    public ValueTask<HttpResponse<List<string>>> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<HttpResponse<List<string>>> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request);
}