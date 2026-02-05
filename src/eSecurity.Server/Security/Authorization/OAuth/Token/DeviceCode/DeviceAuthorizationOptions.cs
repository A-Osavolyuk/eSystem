namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceAuthorizationOptions
{
    public int UserCodeLenght { get; set; } = 8;
    public int DeviceCodeLenght { get; set; } = 32;
    public int Interval { get; set; }
    public TimeSpan Timestamp { get; set; }
    public string VerificationUri { get; set; } = string.Empty;
}