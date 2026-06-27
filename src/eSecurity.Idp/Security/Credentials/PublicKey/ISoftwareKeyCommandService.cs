using eSecurity.Idp.Data.Entities;
using eSecurity.WebAuthN;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public interface ISoftwareKeyCommandService
{
    ValueTask<Result> CreateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> DeleteAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> VerifyAsync(SoftwareKeyEntity entity, PublicKeyCredential credential,
        string savedChallenge, CancellationToken cancellationToken = default);

    ValueTask<Result> ChangeDisplayNameAsync(SoftwareKeyEntity entity, string displayName,
        CancellationToken cancellationToken = default);
}