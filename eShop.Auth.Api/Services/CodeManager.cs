namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICodeManager), ServiceLifetime.Scoped)]
public sealed class CodeManager(AuthDbContext context) : ICodeManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<string> GenerateAsync(UserEntity user, CodeType type,
        CancellationToken cancellationToken = default)
    {
        var code = new Random().Next(100000, 999999).ToString();

        await context.Codes.AddAsync(new VerificationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Code = code,
            Type = type,
            CreateDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == code, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, CodeType type,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Type == type, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, CodeType type,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == user.Id
                                       && x.Code == code
                                       && x.Type == type
                                       && x.ExpireDate < DateTime.UtcNow, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Code not found");
        }

        context.Codes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask DeleteAsync(VerificationCodeEntity entity, CancellationToken cancellationToken = default)
    {
        context.Codes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}