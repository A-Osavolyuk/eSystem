using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShop.Application.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ConfigureDatabaseAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        using var score = app.Services.CreateScope();
        var context = score.ServiceProvider.GetRequiredService<TDbContext>();
        await EnsureDatabaseAsync(context);
        await RunMigrationsAsync(context);
    }

    private static async Task EnsureDatabaseAsync(DbContext context)
    {
        if (!DoesDatabaseExist(context))
        {
            var dbCreator = context.GetService<IRelationalDatabaseCreator>();
            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                if (!await dbCreator.ExistsAsync())
                {
                    await dbCreator.CreateAsync();
                }
            });
        }
    }

    private static async Task RunMigrationsAsync(DbContext context)
    {
        if (!IsDatabaseMigrated(context))
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.MigrateAsync();
                await transaction.CommitAsync();
            });
        }
    }
    
    private static bool IsDatabaseMigrated(DbContext context)
    {
        var pendingMigrations = context.Database.GetPendingMigrations();
        return !pendingMigrations.Any();
    }
    
    private static bool DoesDatabaseExist(DbContext context)
    {
        var dbExists = context.Database.CanConnect();
        return dbExists;
    }
}