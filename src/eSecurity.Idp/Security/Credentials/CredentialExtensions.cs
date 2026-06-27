using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidSafetyNet;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Apple;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.FidoU2F;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.None;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Packed;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;

namespace eSecurity.Idp.Security.Credentials;

public static class CredentialExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddCredentials(Action<CredentialOptions> configure)
        {
            builder.Services.Configure(configure);
            builder.Services.AddScoped<ISoftwareKeyQueryService, SoftwareKeyQueryService>();
            builder.Services.AddScoped<ISoftwareKeyCommandService, SoftwareKeyCommandService>();
            builder.Services.AddScoped<IAttestationProcessor, AttestationProcessor>();
            
            builder.Services.AddSingleton<IAttestationStatementParserProvider, AttestationStatementParserProvider>();
            builder.Services.AddTransient<IAttestationStatementParser, NoneAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, PackedAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, AndroidKeyAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, AndroidSafetyNetAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, FidoU2FAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, AppleAttestationStatementParser>();
            builder.Services.AddTransient<IAttestationStatementParser, TpmAttestationStatementParser>();
            
            builder.Services.AddSingleton<IAttestationStatementVerifierProvider, AttestationStatementVerifierProvider>();
            builder.Services.AddTransient<IAttestationStatementVerifier, NoneAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, PackedAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, AndroidKeyAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, AndroidSafetyNetAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, FidoU2FAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, AppleAttestationStatementVerifier>();
            builder.Services.AddTransient<IAttestationStatementVerifier, TpmAttestationStatementVerifier>();
        }
    }
}