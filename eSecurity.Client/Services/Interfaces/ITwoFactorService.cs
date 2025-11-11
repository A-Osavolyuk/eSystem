using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface ITwoFactorService
{
    public ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request);
    
    public ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    public ValueTask<HttpResponse> RegenerateQrCodeAsync(RegenerateQrCodeRequest request);
    
    public ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
    
    public ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<HttpResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request);
}