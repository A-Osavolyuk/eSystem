namespace eShop.Auth.Api.Services;

public class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;
}