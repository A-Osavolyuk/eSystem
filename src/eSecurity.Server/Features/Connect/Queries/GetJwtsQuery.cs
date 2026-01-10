using System.Security.Cryptography.X509Certificates;
using eSecurity.Server.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetJwksQuery() : IRequest<Result>;

public class GetJwksQueryHandler(
    ICertificateProvider certificateProvider) : IRequestHandler<GetJwksQuery, Result>
{
    private readonly ICertificateProvider _certificateProvider = certificateProvider;

    public async Task<Result> Handle(GetJwksQuery request, CancellationToken cancellationToken)
    {
        var certificate = await _certificateProvider.GetActiveAsync(cancellationToken);
        var publicKey = certificate.Certificate.GetRSAPublicKey();
        if (publicKey is null) return Results.InternalServerError("Error on retrieving public key.");
        
        var securityKey = new RsaSecurityKey(publicKey) { KeyId = certificate.Id.ToString() };
        var jwks = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
        if (jwks is null) return Results.InternalServerError("Error on converting public key.");
        
        jwks.Use = "sig";
        jwks.Alg = SecurityAlgorithms.RsaSha256;

        var response = new JsonWebKeySet() { Keys = { jwks } };
        return Results.Ok(response);
    }
}