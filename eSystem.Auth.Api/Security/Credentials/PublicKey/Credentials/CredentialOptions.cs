namespace eSystem.Auth.Api.Security.Credentials.PublicKey.Credentials;

public class CredentialOptions
{
    public string Domain { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int Timeout { get; set; } = 60000;
}