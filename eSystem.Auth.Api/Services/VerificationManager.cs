using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IVerificationManager), ServiceLifetime.Scoped)]
public class VerificationManager(AuthDbContext context) : IVerificationManager
{
    public async ValueTask<Result> CreateAsync(UserEntity user,
        PurposeType purpose, ActionType action, CancellationToken cancellationToken = default)
    {
        var existedEntity = await context.Verifications.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.Purpose == purpose 
                 && x.Action == action, cancellationToken);

        if (existedEntity is not null) context.Verifications.Remove(existedEntity);
        
        var entity = new VerificationEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Purpose = purpose,
            Action = action,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
            CreateDate = DateTimeOffset.UtcNow
        };

        await context.Verifications.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user,
        PurposeType resource, ActionType action, CancellationToken cancellationToken = default)
    {
        var entity = await context.Verifications.SingleOrDefaultAsync(
            x => x.UserId == user.Id
                 && x.Purpose == resource
                 && x.Action == action, cancellationToken);

        if (entity is null) return Results.NotFound("Verification not found");
        if (entity.ExpireDate < DateTimeOffset.Now) return Results.BadRequest("Verification is expired");

        return Result.Success();
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
            
            context.UserVerificationMethods.Update(preferredMethod);
        }

        var entity = new UserVerificationMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            Preferred = preferred,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.UserVerificationMethods.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserVerificationMethodEntity method,
        CancellationToken cancellationToken = default)
    {
        context.UserVerificationMethods.Remove(method);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> PreferAsync(UserEntity user, VerificationMethod method, 
        CancellationToken cancellationToken = default)
    {
        var currentPreferredMethod = user.VerificationMethods.First(x => x.Preferred);

        if (currentPreferredMethod.Method == method) return Results.BadRequest("Method is already preferred");

        currentPreferredMethod.Preferred = false;
        currentPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;

        var nextPreferredMethod = user.GetVerificationMethod(method);
        
        if (nextPreferredMethod is null) return Results.BadRequest($"User doesn't have verification method {method}.");
        
        nextPreferredMethod.Preferred = true;
        nextPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserVerificationMethods.UpdateRange(currentPreferredMethod, nextPreferredMethod);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}