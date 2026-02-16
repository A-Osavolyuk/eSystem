using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectManager(AuthDbContext context) : IPairwiseSubjectManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PairwiseSubjectEntity?> FindAsync(UserEntity user, ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.PairwiseSubjects.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.ClientId == client.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(PairwiseSubjectEntity subject,
        CancellationToken cancellationToken = default)
    {
        await _context.PairwiseSubjects.AddAsync(subject, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}