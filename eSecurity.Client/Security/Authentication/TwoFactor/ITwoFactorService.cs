using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public interface ITwoFactorService
{
    public ValueTask<Result> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<Result> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<Result> PreferAsync(PreferTwoFactorMethodRequest request);
    
    public ValueTask<Result> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    public ValueTask<Result> RegenerateQrCodeAsync(RegenerateQrCodeRequest request);
    
    public ValueTask<Result> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<Result> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
    
    public ValueTask<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<Result> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request);
}