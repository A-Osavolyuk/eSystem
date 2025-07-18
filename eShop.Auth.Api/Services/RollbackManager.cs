namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRollbackManager), ServiceLifetime.Scoped)]
public class RollbackManager(AuthDbContext context) : IRollbackManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> SaveAsync(UserEntity user, string value, RollbackField field, RollbackAction action,
        CancellationToken cancellationToken)
    {
        var rollback = await context.Rollback.FirstOrDefaultAsync(x => x.UserId == user.Id 
            && x.Field == field && x.Action == action && x.Value == value, cancellationToken);

        if (rollback is not null)
        {
            context.Rollback.Remove(rollback);
        }
        
        var code = GenerateCode();
        var entity = new RollbackEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Code = code,
            Value = user.PasswordHash,
            Action = RollbackAction.Reset,
            Field = RollbackField.Password,
            CreateDate = DateTimeOffset.UtcNow,
        };
        
        await context.Rollback.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    private string GenerateCode()
    {
        var random = new Random();
        var code = random.Next(100_000, 999_999).ToString();
        
        return code;
    }
}