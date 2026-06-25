using eSecurity.Idp.Data.Entities;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Codes;

public interface ICodeCommandService
{
    ValueTask<TypedResult<string>> CreateAsync(Guid userId, SenderType sender,
        CancellationToken cancellationToken = default);

    ValueTask<Result> ConsumeAsync(CodeEntity code, CancellationToken cancellationToken = default);

    ValueTask<Result> CancelAsync(CodeEntity code, CancellationToken cancellationToken = default);
}