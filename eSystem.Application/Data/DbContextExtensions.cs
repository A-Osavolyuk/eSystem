using eSystem.Domain.Abstraction.Data.Seeding;
using Microsoft.EntityFrameworkCore;

namespace eSystem.Application.Data;

public static class DbContextExtensions
{
    public static async Task SeedAsync<TMarker>(this DbContext context,
        CancellationToken cancellationToken = default)
    {
        var baseType = typeof(Seed<>);
        var assembly = typeof(TMarker).Assembly;

        var implementations = assembly
            .GetTypes()
            .Where(t =>
                t is
                {
                    IsClass: true, IsAbstract: false, IsGenericType: false,
                    BaseType: { IsGenericType: true, IsAbstract: true }
                } && t.BaseType.GetGenericTypeDefinition() == baseType)
            .ToList();

        foreach (var implType in implementations)
        {
            var instance = Activator.CreateInstance(implType);
            if (instance is null)
                continue;

            var method = implType.GetMethod("Get");
            var result = method?.Invoke(instance, null);
            if (result is not IEnumerable<object> entities)
                continue;

            var entityType = implType.BaseType!.GetGenericArguments().First();
            if (!await AnyAsync(context, entityType, cancellationToken))
            {
                await AddRangeAsync(context, entityType, entities, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<bool> AnyAsync(DbContext context, 
        Type entityType, CancellationToken cancellationToken)
    {
        var dbSet = context
            .GetType()
            .GetMethod("Set", Type.EmptyTypes)!
            .MakeGenericMethod(entityType)
            .Invoke(context, null);
        
        var anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .First(m => m.Name == "AnyAsync" && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        var queryable = dbSet as IQueryable;
        var task = (Task)anyAsyncMethod.Invoke(null, [queryable!, cancellationToken])!;
        await task.ConfigureAwait(false);

        return (bool)(task.GetType().GetProperty("Result")?.GetValue(task) ?? false);
    }

    private static async Task AddRangeAsync(DbContext context, Type entityType, IEnumerable<object> entities,
        CancellationToken cancellationToken)
    {
        Type[] types = [typeof(IEnumerable<>).MakeGenericType(entityType), typeof(CancellationToken)];
        var method = typeof(DbContext).GetMethod("AddRangeAsync", types);
        
        if (method is null)
        {
            foreach (var entity in entities)
            {
                await context.AddAsync(entity, cancellationToken);
            }
        }
        else
        {
            await (Task)method.Invoke(context, [entities, cancellationToken])!;
        }
    }
}