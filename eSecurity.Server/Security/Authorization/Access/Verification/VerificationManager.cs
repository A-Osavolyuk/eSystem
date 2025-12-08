using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Access.Verification;

public class VerificationManager(AuthDbContext context) : IVerificationManager
{
    private readonly AuthDbContext _context = context;
    
    public async ValueTask<UserVerificationMethodEntity?> GetAsync(UserEntity user, 
        VerificationMethod method, CancellationToken cancellationToken = default)
    {
        return await _context.UserVerificationMethods.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Method == method, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserEntity user,
        PurposeType purpose, ActionType action, CancellationToken cancellationToken = default)
    {
        var existedEntity = await _context.Verifications.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.Purpose == purpose 
                 && x.Action == action, cancellationToken);

        if (existedEntity is not null) _context.Verifications.Remove(existedEntity);
        
        var entity = new VerificationEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Purpose = purpose,
            Action = action,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
            CreateDate = DateTimeOffset.UtcNow
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

    public async ValueTask<Result> SubscribeAsync(UserEntity user, VerificationMethod method, 
        bool preferred = false, CancellationToken cancellationToken = default)
    {
        if (user.HasVerification(method))
            return Results.BadRequest("Verification method is already subscribed");

        if (preferred && user.VerificationMethods.Any(x => x.Preferred))
        {
            var preferredMethod = user.VerificationMethods.First(x => x.Preferred);
            preferredMethod.Preferred = false;
            preferredMethod.UpdateDate = DateTimeOffset.UtcNow;
            
            _context.UserVerificationMethods.Update(preferredMethod);
        }

        var entity = new UserVerificationMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            Preferred = preferred,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await _context.UserVerificationMethods.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserVerificationMethodEntity method,
        CancellationToken cancellationToken = default)
    {
        _context.UserVerificationMethods.Remove(method);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> PreferAsync(UserEntity user, VerificationMethod method, 
        CancellationToken cancellationToken = default)
    {
        var currentPreferredMethod = user.VerificationMethods.First(x => x.Preferred);

        if (currentPreferredMethod.Method == method) return Results.BadRequest("Method is already preferred");

        currentPreferredMethod.Preferred = false;
        currentPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;

        var nextPreferredMethod = await _context.UserVerificationMethods.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Method == method, cancellationToken);
        
        if (nextPreferredMethod is null) return Results.BadRequest($"User doesn't have verification method {method}.");
        
        nextPreferredMethod.Preferred = true;
        nextPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.UserVerificationMethods.UpdateRange(currentPreferredMethod, nextPreferredMethod);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}