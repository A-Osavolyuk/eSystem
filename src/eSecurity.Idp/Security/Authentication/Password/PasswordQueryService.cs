using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Password;

public sealed class PasswordQueryService(AuthDbContext context) : IPasswordQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PasswordEntity?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Passwords.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Passwords.AnyAsync(x => x.UserId == userId, cancellationToken);
    }
}