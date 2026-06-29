using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public sealed class PublicSubjectCommandService(AuthDbContext context) : IPublicSubjectCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(PublicSubjectEntity entity, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        await _context.PublicSubjects.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}