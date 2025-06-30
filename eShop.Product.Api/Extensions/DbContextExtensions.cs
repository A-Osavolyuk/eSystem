using eShop.Product.Api.Data.Seed;

namespace eShop.Product.Api.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedAsync(this AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Categories.AnyAsync(cancellationToken))
        {
            var seed = new CategorySeed();

            await context.Categories.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        if (!await context.Types.AnyAsync(cancellationToken))
        {
            var seed = new TypeSeed();

            await context.Types.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        if (!await context.Units.AnyAsync(cancellationToken))
        {
            var seed = new UnitSeed();

            await context.Units.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        if (!await context.PriceType.AnyAsync(cancellationToken))
        {
            var seed = new PriceTypeSeed();

            await context.PriceType.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}