using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Cryptography.Keys.PrivateKey;

public class RsaKeyProvider(
    IKeyFactory keyFactory,
    IDataProtectionProvider protectionProvider) : IKeyProvider
{
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly IDataProtectionProvider protectionProvider = protectionProvider;

    public async Task<RSA?> GetPublicKeyAsync()
    {
        var certificate = await GetCertificateAsync();
        return certificate.GetRSAPrivateKey();
    }

    public async Task<RSA?> GetPrivateKeyAsync()
    {
        var certificate = await GetCertificateAsync();
        return certificate.GetRSAPublicKey();
    }

    private async Task<X509Certificate2> GetCertificateAsync()
    {
        if (!File.Exists("certPassword.dat") || !File.Exists("cert.pfx"))
        {
            var rsa = RSA.Create(2048);

            var request = new CertificateRequest(
                "CN=JwtSigningKey",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var password = keyFactory.Create(20);
            var passwordProtector = protectionProvider.CreateProtector(ProtectionPurposes.Password);
            var protectedPassword = passwordProtector.Protect(Encoding.UTF8.GetBytes(password));
            await File.WriteAllBytesAsync("certPassword.dat", protectedPassword);

            var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddYears(1));

            var certificateBytes = certificate.Export(X509ContentType.Pkcs12, password);
            await File.WriteAllBytesAsync("cert.pfx", certificateBytes);

            return certificate;
        }
        else
        {
            if (!File.Exists("certPassword.dat"))
                throw new FileNotFoundException("RSA key password does not exist.");

            var protectedPassword = await File.ReadAllBytesAsync("certPassword.dat");
            var protector = protectionProvider.CreateProtector(ProtectionPurposes.Password);
            var passwordBytes = protector.Unprotect(protectedPassword);
            var password = Encoding.UTF8.GetString(passwordBytes);

            if (!File.Exists("cert.pfx"))
                throw new FileNotFoundException("RSA key does not exist.");

            var pfxCertificateBytes = await File.ReadAllBytesAsync("cert.pfx");
            var certificate = X509CertificateLoader.LoadPkcs12(
                pfxCertificateBytes,
                password,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);

            return certificate;
        }
    }
}