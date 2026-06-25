using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Codes;

public interface ICodeQueryService
{
    ValueTask<CodeEntity?> GetByCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}