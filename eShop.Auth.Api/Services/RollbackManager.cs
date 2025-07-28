namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRollbackManager), ServiceLifetime.Scoped)]
public sealed class RollbackManager(AuthDbContext context) : IRollbackManager
{
    private readonly AuthDbContext context = context;
}