using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Consents;

public interface IConsentQueryService
{
    ValueTask<ConsentEntity?> GetByClientAsync(Guid userId, Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<ConsentEntity?> GetByIdAsync(Guid consentId, CancellationToken cancellationToken = default);
}