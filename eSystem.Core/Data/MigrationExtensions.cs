using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Data;

public static class MigrationExtensions
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
        if (!await DoesDatabaseExistAsync(context))
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
        if (!await IsDatabaseMigratedAsync(context))
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
    
    private static async Task<bool> IsDatabaseMigratedAsync(DbContext context)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        return !pendingMigrations.Any();
    }
    
    private static async Task<bool> DoesDatabaseExistAsync(DbContext context)
    {
        var dbExists = await context.Database.CanConnectAsync();
        return dbExists;
    }
}