using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Data;

public static class MigrationExtensions
{
    extension(WebApplication app)
    {
        public async Task ConfigureDatabaseAsync<TDbContext>() where TDbContext : DbContext
        {
            using var score = app.Services.CreateScope();
            var context = score.ServiceProvider.GetRequiredService<TDbContext>();
            await EnsureDatabaseAsync(context);
            await RunMigrationsAsync(context);
        }
    }

    private static async Task EnsureDatabaseAsync(DbContext context)
    {
        var dbCreator = context.GetService<IRelationalDatabaseCreator>();
        if (!await context.Database.CanConnectAsync())
        {
            await dbCreator.CreateAsync();
        }
    }

    private static async Task RunMigrationsAsync(DbContext context)
    {
        if (!await IsDatabaseMigratedAsync(context))
        {
            await context.Database.MigrateAsync();
        }
    }
    
    private static async Task<bool> IsDatabaseMigratedAsync(DbContext context)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        return !pendingMigrations.Any();
    }
}