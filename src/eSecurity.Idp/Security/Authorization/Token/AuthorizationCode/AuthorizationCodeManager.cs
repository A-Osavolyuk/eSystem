using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Token.AuthorizationCode;

public class AuthorizationCodeManager(AuthDbContext context) : IAuthorizationCodeManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<AuthorizationCodeEntity?> FindByCodeAsync(string code, 
        CancellationToken cancellationToken = default)
    {
        return await _context.AuthorizationCodes
            .Where(c => c.Code == code)
            .Include(x => x.Session)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        await _context.AuthorizationCodes.AddAsync(code, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UseAsync(AuthorizationCodeEntity code, 
        CancellationToken cancellationToken = default)
    {
        code.Used = true;
        
        _context.AuthorizationCodes.Update(code);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}