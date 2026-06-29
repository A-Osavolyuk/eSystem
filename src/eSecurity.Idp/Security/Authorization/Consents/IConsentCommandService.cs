using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Consents;

public interface IConsentCommandService
{
    ValueTask<Result> CreateAsync(ConsentEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> GrantScopesAsync(Guid consentId, IEnumerable<string> scopes,
        CancellationToken cancellationToken = default);
}