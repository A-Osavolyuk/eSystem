using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailCommandService
{
    ValueTask<Result> AddAsync(Guid userId, string email, EmailType type,
        CancellationToken cancellationToken = default);

    ValueTask<Result> ChangeAsync(Guid userId, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);
    
    ValueTask<Result> ResetAsync(Guid userId, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);

    ValueTask<Result> VerifyAsync(Guid userId, string email, 
        CancellationToken cancellationToken = default);

    ValueTask<Result> RemoveAsync(Guid userId, string email, 
        CancellationToken cancellationToken = default);
}