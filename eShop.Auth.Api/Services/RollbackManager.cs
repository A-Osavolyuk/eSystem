namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRollbackManager), ServiceLifetime.Scoped)]
public class RollbackManager(AuthDbContext context) : IRollbackManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<RollbackEntity?> CommitAsync(UserEntity user, string value, 
        RollbackField field, CancellationToken cancellationToken = default)
    {
        var rollback = await context.Rollback.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Field == field, cancellationToken);

        if (rollback is not null)
        {
            context.Rollback.Remove(rollback);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var code = CodeGenerator.Generate(6);
        var entity = new RollbackEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Code = code,
            Value = user.PasswordHash,
            Field = RollbackField.Password,
            CreateDate = DateTimeOffset.UtcNow,
        };
        
        await context.Rollback.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async ValueTask<Result> RollbackAsync(UserEntity user, string code, RollbackField field,
        CancellationToken cancellationToken = default)
    {
        var rollback = await context.Rollback.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Field == field && x.Code == code, cancellationToken);

        if (rollback is null)
        {
            return Results.NotFound($"Cannot rollback {field}, rollback was not found");
        }

        switch (field)
        {
            case RollbackField.Password:
            {
                user.PasswordHash = rollback.Value;
                user.PasswordChangeDate = DateTimeOffset.UtcNow;
                
                break;
            }
            case RollbackField.Email:
            {
                user.Email = rollback.Value;
                user.EmailChangeDate = DateTimeOffset.UtcNow;
                
                break;
            }
            case RollbackField.RecoveryEmail:
            {
                user.RecoveryEmail = rollback.Value;
                user.RecoveryEmailChangeDate = DateTimeOffset.UtcNow;
                
                break;
            }
            case RollbackField.PhoneNumber:
            {
                user.PhoneNumber = rollback.Value;
                user.PhoneNumberChangeDate = DateTimeOffset.UtcNow;
                
                break;
            }
            default: throw new NotSupportedException("Rollback field is not supported");
        }
        
        user.UpdateDate = DateTimeOffset.UtcNow;
        
        context.Users.Update(user);
        context.Rollback.Remove(rollback);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}