using eSystem.Domain.Common.Results;
using eSystem.Product.Api.Data;
using eSystem.Product.Api.Entities;
using eSystem.Product.Api.Interfaces;

namespace eSystem.Product.Api.Services;

[Injectable(typeof(IProductManager), ServiceLifetime.Scoped)]
public class ProductManager(AppDbContext context) : IProductManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<Result> CreateAsync(ProductEntity entity, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(ProductEntity entity, CancellationToken cancellationToken = default)
    {
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(ProductEntity entity, CancellationToken cancellationToken = default)
    {
        context.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}