using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Cryptography.Keys;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Cryptography.Signing.Certificates;

public class CertificateHandler(
    IDataProtectionProvider protection,
    IKeyFactory keyFactory,
    IOptions<CertificateOptions> options) : ICertificateHandler
{
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly CertificateOptions _options = options.Value;
    private readonly IDataProtector _passwordProtector = protection.CreateProtector(ProtectionPurposes.Password);
    private readonly IDataProtector _certificateProtector = protection.CreateProtector(ProtectionPurposes.Certificate);
    
    public ProtectedCertificate CreateCertificate()
    {
        using var rsa = RSA.Create(_options.KeyLength * 8);

        var request = new CertificateRequest(
            _options.SubjectName,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
        
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(1)),
            DateTimeOffset.UtcNow.Add(_options.CertificateLifetime));

        var password = _keyFactory.Create(20);
        var certificateBytes = certificate.Export(X509ContentType.Pkcs12, password);

        return new ProtectedCertificate()
        {
            ProtectedCertificateBytes = _certificateProtector.Protect(certificateBytes),
            ProtectedPasswordBytes = _passwordProtector.Protect(Encoding.UTF8.GetBytes(password)),
            Certificate = certificate
        };
    }

    public X509Certificate2 ExportCertificate(byte[] protectedCertificate, byte[] protectedPassword)
    {
        var passwordBytes = _passwordProtector.Unprotect(protectedPassword);
        var password = Encoding.UTF8.GetString(passwordBytes);
        var certificateBytes = _certificateProtector.Unprotect(protectedCertificate);
        
        try
        {
            return X509CertificateLoader.LoadPkcs12(
                certificateBytes,
                password,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Failed to load certificate.", ex);
        }
    }
}