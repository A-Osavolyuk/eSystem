using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class UserQueryService(AuthDbContext context) : IUserQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async ValueTask<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var normalizedUsername = Normalizer.Normalize(username);
        return await _context.Users.FirstOrDefaultAsync(
            x => x.NormalizedUsername == normalizedUsername, cancellationToken);
    }

    public async ValueTask<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = Normalizer.Normalize(email);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => _context.UserEmails
                    .Any(e => e.UserId == u.Id &&
                              e.Type == EmailType.Primary &&
                              e.NormalizedEmail == normalizedEmail
                    ), cancellationToken
            );

        return user;
    }

    public async ValueTask<UserEntity?> GetBySubjectAsync(string subject, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var user = await _context.PublicSubjects
            .Where(x => x.Subject == subject)
            .Select(x => x.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is not null)
            return user;

        return await _context.PairwiseSubjects
            .Where(x => x.Subject == subject)
            .Select(x => x.User)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<UserEntity?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);

        var normalizedValue = Normalizer.Normalize(login);
        return await _context.Users
            .Where(user => user.NormalizedUsername == normalizedValue ||
                           user.Emails.Any(x =>
                               x.NormalizedEmail == normalizedValue &&
                               x.Type == EmailType.Primary
                           )
            ).FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var normalizedUsername = Normalizer.Normalize(username);
        return await _context.Users.AnyAsync(x => x.NormalizedUsername == normalizedUsername, cancellationToken);
    }
}