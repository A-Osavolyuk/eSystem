using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Public;

public sealed class PublicSubjectManager(AuthDbContext context) : IPublicSubjectManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PublicSubjectEntity?> FindAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        return await _context.PublicSubjects.FirstOrDefaultAsync(
            p => p.UserId == user.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(PublicSubjectEntity subject,
        CancellationToken cancellationToken = default)
    {
        await _context.PublicSubjects.AddAsync(subject, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}