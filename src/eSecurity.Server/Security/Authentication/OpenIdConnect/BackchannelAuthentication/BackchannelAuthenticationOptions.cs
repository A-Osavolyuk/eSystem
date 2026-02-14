namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class BackchannelAuthenticationOptions
{
    public int Interval { get; set; }
    public int AuthReqIdLength { get; set; }
    public int UserCodeMaxLength { get; set; }
    public int UserCodeMinLength { get; set; }
    public TimeSpan DefaultRequestLifetime { get; set; }
    public TimeSpan MinRequestLifetime { get; set; }
    public TimeSpan MaxRequestLifetime { get; set; }
}