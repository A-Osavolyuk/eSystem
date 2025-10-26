using eSystem.Auth.Api.Security.Cryptography.Keys;

namespace eSystem.Auth.Api.Security.Authentication.SSO.Code;

public class AuthorizationCodeManager(
    AuthDbContext context,
    IKeyFactory keyFactory) : IAuthorizationCodeManager
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;

    public async ValueTask<AuthorizationCodeEntity?> FindByCodeAsync(string code, 
        CancellationToken cancellationToken = default)
    {
        return await context.AuthorizationCodes
            .Include(x => x.Client)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        await context.AuthorizationCodes.AddAsync(code, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UseAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        code.Used = true;
        code.UpdateDate = DateTimeOffset.UtcNow;
        
        context.AuthorizationCodes.Update(code);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public string Generate() => keyFactory.Create(20);
}