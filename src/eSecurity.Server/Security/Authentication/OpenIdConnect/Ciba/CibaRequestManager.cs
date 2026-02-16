using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Ciba;

public sealed class CibaRequestManager(AuthDbContext context) : ICibaRequestManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<CibaRequestEntity?> FindByAuthReqIdAsync(string authReqId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CibaRequests.FirstOrDefaultAsync(
            x => x.AuthReqId == authReqId, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(CibaRequestEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.CibaRequests.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(CibaRequestEntity entity, CancellationToken cancellationToken = default)
    {
        _context.CibaRequests.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}