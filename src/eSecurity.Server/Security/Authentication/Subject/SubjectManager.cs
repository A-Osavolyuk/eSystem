using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class SubjectManager(AuthDbContext context) : ISubjectManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreatePublicSubjectAsync(PublicSubjectEntity subject,
        CancellationToken cancellationToken = default)
    {
        await _context.PublicSubjects.AddAsync(subject, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> CreatePairwiseSubjectAsync(PairwiseSubjectEntity subject,
        CancellationToken cancellationToken = default)
    {
        await _context.PairwiseSubjects.AddAsync(subject, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}