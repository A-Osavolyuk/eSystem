namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICodeManager), ServiceLifetime.Scoped)]
public sealed class CodeManager(AuthDbContext context) : ICodeManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, CodeType type, CodeResource resource,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .FirstOrDefaultAsync(x => x.UserId == user.Id 
                                      && x.Type == type 
                                      && x.Sender == sender, cancellationToken);

        if (entity is not null)
        {
            context.Codes.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        var code = new Random().Next(100000, 999999).ToString();

        await context.Codes.AddAsync(new CodeEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Code = code,
            Type = type,
            Sender = sender,
            Resource = resource,
            CreateDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<CodeEntity?> FindAsync(UserEntity user, SenderType sender, CodeType type,
        CodeResource resource, CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .FirstOrDefaultAsync(c => c.UserId == user.Id 
                                      && c.Type == type 
                                      && c.Sender == sender 
                                      && c.Resource == resource, cancellationToken);

        return entity;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, CodeType type,
        CodeResource resource, CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .SingleOrDefaultAsync(x => x.UserId == user.Id
                                       && x.Code == code
                                       && x.Type == type
                                       && x.Sender == sender
                                       && x.Resource == resource
                                       && x.ExpireDate > DateTime.UtcNow, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Code not found");
        }

        context.Codes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}