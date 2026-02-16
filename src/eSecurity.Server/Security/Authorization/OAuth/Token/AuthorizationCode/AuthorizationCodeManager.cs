using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class AuthorizationCodeManager(
    AuthDbContext context,
    IKeyFactory keyFactory) : IAuthorizationCodeManager
{
    private readonly AuthDbContext _context = context;
    private readonly IKeyFactory _keyFactory = keyFactory;

    public async ValueTask<AuthorizationCodeEntity?> FindByCodeAsync(string code, 
        CancellationToken cancellationToken = default)
    {
        return await _context.AuthorizationCodes.FirstOrDefaultAsync(
            c => c.Code == code, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        await _context.AuthorizationCodes.AddAsync(code, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> UseAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        code.Used = true;
        
        _context.AuthorizationCodes.Update(code);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public string Generate() => _keyFactory.Create(20);
}