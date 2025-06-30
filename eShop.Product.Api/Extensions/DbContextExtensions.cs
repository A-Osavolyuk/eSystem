using eShop.Product.Api.Data.Seed;

namespace eShop.Product.Api.Extensions;

public static class DbContextExtensions
{
    public static async Task SeedAsync(this AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Types.AnyAsync(cancellationToken))
        {
            var seed = new TypeSeed();

            await context.Types.AddRangeAsync(seed.Get(), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }}