using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidSafetyNet;

public sealed class AndroidSafetyNetAttestationStatementVerifier
    : AttestationStatementVerifier<AndroidSafetyNetAttestationStatement>
{
    public override AttestationFormatType Format => AttestationFormatType.AndroidSafetyNet;

    public override async ValueTask<AttestationVerificationResult> VerifyAsync(
        AuthenticationData authenticationData,
        AndroidSafetyNetAttestationStatement attestationStatement,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default)
    {
        var nonceBytes = SHA256.HashData(authenticationData.Raw.Concat(clientDataHash).ToArray());
        var expectedNonce = Base64UrlEncoder.Encode(nonceBytes);

        if (!VerifyJwtSignatureWithGoogleCerts(attestationStatement.Response))
            return AttestationVerificationResult.Fail();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(attestationStatement.Response);

        var jwtNonce = jwt.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value;
        if (jwtNonce is null || !string.Equals(jwtNonce, expectedNonce, StringComparison.Ordinal))
            return AttestationVerificationResult.Fail();

        var basic = bool.Parse(jwt.Claims.First(c => c.Type == "basicIntegrity").Value);
        var cts = bool.Parse(jwt.Claims.First(c => c.Type == "ctsProfileMatch").Value);

        if (!basic)
            return AttestationVerificationResult.Fail();

        return new AttestationVerificationResult
        {
            IsValid = true,
            TrustType = cts
                ? AttestationTrustType.Trusted
                : AttestationTrustType.Untrusted
        };
    }

    private static bool VerifyJwtSignatureWithGoogleCerts(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        if (token.Header["x5c"] is not JsonArray x5c || x5c.Count == 0)
            return false;

        var certBytes = Convert.FromBase64String(x5c[0]!.ToString());
        var cert = X509CertificateLoader.LoadCertificate(certBytes);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://www.android.com/attestation",
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new X509SecurityKey(cert),
            RequireSignedTokens = true
        };

        try
        {
            handler.ValidateToken(jwt, parameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}