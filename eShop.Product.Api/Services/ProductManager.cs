using eShop.Product.Api.Interfaces;

namespace eShop.Product.Api.Services;

public class ProductManager(AppDbContext context) : IProductManager
{
    private readonly AppDbContext context = context;

    public async ValueTask<Result> CreateAsync(ProductEntity entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(ProductEntity entity)
    {
        context.Update(entity);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(ProductEntity entity)
    {
        context.Remove(entity);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }
}