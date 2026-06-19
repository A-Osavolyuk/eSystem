using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.Email;

public sealed class EmailQueryService(AuthDbContext context) : IEmailQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserEmailEntity>> ListByUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<UserEmailEntity>> ListByTypeAsync(Guid userId, EmailType emailType,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == userId && x.Type == emailType)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> GetByEmailAsync(Guid userId, string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        var normalizedEmail = Normalizer.Normalize(email);
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == userId && x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> GetByTypeAsync(Guid userId, EmailType type, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == userId && x.Type == type, cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> FindByEmailAsync(string email, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = Normalizer.Normalize(email);
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = Normalizer.Normalize(email);
        return await _context.UserEmails.AnyAsync(
            x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}