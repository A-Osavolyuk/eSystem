using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Access.Verification;

public class VerificationManager(AuthDbContext context) : IVerificationManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(UserEntity user,
        PurposeType purpose, ActionType action, CancellationToken cancellationToken = default)
    {
        var existedEntity = await _context.Verifications.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.Purpose == purpose 
                 && x.Action == action, cancellationToken);

        if (existedEntity is not null) _context.Verifications.Remove(existedEntity);
        
        var entity = new VerificationEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Purpose = purpose,
            Action = action,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        await _context.Verifications.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user,
        PurposeType resource, ActionType action, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Verifications.SingleOrDefaultAsync(
            x => x.UserId == user.Id
                 && x.Purpose == resource
                 && x.Action == action, cancellationToken);

        if (entity is null) return Results.NotFound("Verification not found");
        if (entity.ExpireDate < DateTimeOffset.Now) return Results.BadRequest("Verification is expired");

        return Results.Ok();
    }
}