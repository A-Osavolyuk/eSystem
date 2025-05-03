using eShop.Auth.Api.Data.Seed;

namespace eShop.Auth.Api.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedAsync(this AuthDbContext dbContext, DbContext ctx, bool isStoreOperation = false,
        CancellationToken cancellationToken = default)
    {
        var context = (ctx as AuthDbContext)!;

        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var seed = new UserSeed();

            await context.Users.AddRangeAsync(seed.Get(), cancellationToken);
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
                
        if (!await context.PersonalData.AnyAsync(cancellationToken))
        {
            var seed = new PersonalDataSeed();

            await context.PersonalData.AddRangeAsync(seed.Get(), cancellationToken);
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
    }}