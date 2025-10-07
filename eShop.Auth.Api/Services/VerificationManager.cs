namespace eShop.Auth.Api.Services;

[Injectable(typeof(IVerificationManager), ServiceLifetime.Scoped)]
public class VerificationManager(AuthDbContext context) : IVerificationManager
{
    public async ValueTask<Result> CreateAsync(UserEntity user,
        CodeResource resource, CodeType type, CancellationToken cancellationToken = default)
    {
        var entity = new VerificationEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Resource = resource,
            Type = type,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
            CreateDate = DateTimeOffset.UtcNow
        };

        await context.Verifications.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user,
        CodeResource resource, CodeType type, CancellationToken cancellationToken = default)
    {
        var entity = await context.Verifications.SingleOrDefaultAsync(
            x => x.UserId == user.Id
                 && x.Resource == resource
                 && x.Type == type, cancellationToken);

        if (entity is null) return Results.NotFound("Verification not found");
        if (entity.ExpireDate < DateTimeOffset.Now) return Results.BadRequest("Verification is expired");

        context.Verifications.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, VerificationMethod method, 
        bool isPrimary = false, CancellationToken cancellationToken = default)
    {
        if (user.HasVerificationMethod(method))
            return Results.BadRequest("Verification method is already subscribed");

        var entity = new UserVerificationMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            IsPrimary = isPrimary,
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
}