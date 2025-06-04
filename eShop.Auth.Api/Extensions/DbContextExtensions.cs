using eShop.Auth.Api.Data.Seed;

namespace eShop.Auth.Api.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedAsync(this AuthDbContext context, bool isStoreOperation = false,
        CancellationToken cancellationToken = default)
    {
        if (!await context.PersonalData.AnyAsync(cancellationToken))
        {
            var seed = new PersonalDataSeed();

            await context.PersonalData.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (!await context.Providers.AnyAsync(cancellationToken))
        {
            var seed = new ProviderSeed();

            await context.Providers.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var seed = new UserSeed();

            await context.Users.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.UserProvider.AnyAsync(cancellationToken))
        {
            var seed = new UserProviderSeed();
            
            await context.UserProvider.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (!await context.Resources.AnyAsync(cancellationToken))
        {
            var seed = new ResourceSeed();

            await context.Resources.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Permissions.AnyAsync(cancellationToken))
        {
            var seed = new PermissionSeed();

            await context.Permissions.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
                
        if (!await context.Roles.AnyAsync(cancellationToken))
        {
            var seed = new RoleSeed();

            await context.Roles.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
                
        if (!await context.UserRoles.AnyAsync(cancellationToken))
        {
            var seed = new UserRoleSeed();

            await context.UserRoles.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
                
        if (!await context.UserPermissions.AnyAsync(cancellationToken))
        {
            var seed = new UserPermissionSeed();

            await context.UserPermissions.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (!await context.RolePermissions.AnyAsync(cancellationToken))
        {
            var seed = new RolePermissionSeed();

            await context.RolePermissions.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (!await context.LockoutState.AnyAsync(cancellationToken))
        {
            var seed = new LockoutSeed();

            await context.LockoutState.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }}