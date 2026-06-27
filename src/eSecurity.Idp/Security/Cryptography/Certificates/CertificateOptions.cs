namespace eSecurity.Idp.Security.Cryptography.Certificates;

public class CertificateOptions
{
    public string SubjectName { get; set; } = string.Empty;
    public TimeSpan CertificateLifetime { get; set; }
    public int KeyLength { get; set; }
}