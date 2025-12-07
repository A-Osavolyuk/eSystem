using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Email;

public class EmailManager(AuthDbContext context) : IEmailManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, EmailType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == user.Id && x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> FindByTypeAsync(UserEntity user, EmailType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Type == type, cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> FindByEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == email, cancellationToken);
    }
}