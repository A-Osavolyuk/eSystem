namespace eSecurity.Security.Cryptography.Keys.SigningKey;

public class SigningKeyOptions
{
    public string SubjectName { get; set; } = string.Empty;
    public TimeSpan CertificateLifetime { get; set; }
    public int KeyLength { get; set; }
}