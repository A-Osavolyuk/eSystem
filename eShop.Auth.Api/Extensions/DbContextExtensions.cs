using System.Reflection;
using eShop.Auth.Api.Data.Seeding;
using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedAsync<TEntity>(this AuthDbContext context, CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        var baseType = typeof(Seed<Entity>);
        var assembly = context.GetType().Assembly;
        
        var implementations = assembly
            .GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsInterface: false, IsClass: true, BaseType.IsGenericType: true } &&
                t.BaseType.GetGenericTypeDefinition() == baseType);

        foreach (var implType in implementations)
        {
            var seedInstance = Activator.CreateInstance(implType) as Seed<TEntity>;

            if (seedInstance is null)
            {
                continue;
            }

            var entities = seedInstance.Get();

            if (!await context.AnyExistsAsync(entities[0], cancellationToken))
            {
                await context.AddRangeAsync(entities, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<bool> AnyExistsAsync<TEntity>(this AuthDbContext context, TEntity entity,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        
        return await dbSet.AnyAsync(cancellationToken);
    }
    
    public static async Task SeedAsync(this AuthDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Providers.AnyAsync(cancellationToken))
        {
            var seed = new ProviderSeed();
            await context.Providers.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var seed = new UserSeed();
            await context.Users.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.PersonalData.AnyAsync(cancellationToken))
        {
            var seed = new PersonalDataSeed();
            await context.PersonalData.AddRangeAsync(seed.Get(), cancellationToken);
        }

        if (!await context.UserProvider.AnyAsync(cancellationToken))
        {
            var seed = new UserProviderSeed();
            await context.UserProvider.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.ResourceOwners.AnyAsync(cancellationToken))
        {
            var seed = new ResourceOwnerSeed();
            await context.ResourceOwners.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.Resources.AnyAsync(cancellationToken))
        {
            var seed = new ResourceSeed();
            await context.Resources.AddRangeAsync(seed.Get(), cancellationToken);
        }

        if (!await context.Permissions.AnyAsync(cancellationToken))
        {
            var seed = new PermissionSeed();
            await context.Permissions.AddRangeAsync(seed.Get(), cancellationToken);
        }
                
        if (!await context.Roles.AnyAsync(cancellationToken))
        {
            var seed = new RoleSeed();
            await context.Roles.AddRangeAsync(seed.Get(), cancellationToken);
        }
                
        if (!await context.UserRoles.AnyAsync(cancellationToken))
        {
            var seed = new UserRoleSeed();
            await context.UserRoles.AddRangeAsync(seed.Get(), cancellationToken);
        }
                
        if (!await context.UserPermissions.AnyAsync(cancellationToken))
        {
            var seed = new UserPermissionSeed();
            await context.UserPermissions.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.RolePermissions.AnyAsync(cancellationToken))
        {
            var seed = new RolePermissionSeed();
            await context.RolePermissions.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        if (!await context.LockoutStates.AnyAsync(cancellationToken))
        {
            var seed = new LockoutSeed();
            await context.LockoutStates.AddRangeAsync(seed.Get(), cancellationToken);
        }
        if (!await context.LockoutReasons.AnyAsync(cancellationToken))
        {
            var seed = new LockoutReasonSeed();
            await context.LockoutReasons.AddRangeAsync(seed.Get(), cancellationToken);
        }
        
        await context.SaveChangesAsync(cancellationToken);
    }}