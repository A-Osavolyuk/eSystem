using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Scopes;

public class ScopeManager(AuthDbContext context) : IScopeManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<ScopeEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Scopes.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}