using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectCommandService(AuthDbContext context) : IPairwiseSubjectCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(PairwiseSubjectEntity entity, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.PairwiseSubjects.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}