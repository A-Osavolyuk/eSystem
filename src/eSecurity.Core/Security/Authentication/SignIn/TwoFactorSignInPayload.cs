using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Security.Authentication.SignIn;

public class TwoFactorSignInPayload : SignInPayload
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }
    
    [JsonPropertyName("payload")]
    public required TwoFactorPayload Payload { get; set; }
}