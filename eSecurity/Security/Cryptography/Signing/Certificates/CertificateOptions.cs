namespace eSecurity.Security.Cryptography.Signing.Certificates;

public class CertificateOptions
{
    public string SubjectName { get; set; } = string.Empty;
    public TimeSpan CertificateLifetime { get; set; }
    public int KeyLength { get; set; }
}