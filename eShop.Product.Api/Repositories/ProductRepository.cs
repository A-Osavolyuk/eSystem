using eShop.Domain.DTOs;
using eShop.Domain.Requests.Api.Product;
using eShop.Product.Api.Entities;

namespace eShop.Product.Api.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    private readonly AppDbContext context = context;

    public async ValueTask<IEnumerable<ProductDto>> GetProductsAsync()
    {
        return await context.Products
            .AsNoTracking()
            .Select(x => Mapper.ToProductDto(x))
            .ToListAsync();
    }

    public async ValueTask<IEnumerable<ProductDto>> GetProductsByTypeAsync(ProductTypes productType)
    {
        return await context.Products
            .AsNoTracking()
            .Where(x => x.ProductType == productType)
            .Select(x => Mapper.ToProductDto(x))
            .ToListAsync();
    }

    public async ValueTask<IEnumerable<ProductDto>> FindProductsByNameAsync(string name)
    {
        return await context.Products
            .AsNoTracking()
            .Where(x => x.Name.Contains(name))
            .Select(x => Mapper.ToProductDto(x))
            .ToListAsync();
    }

    public async ValueTask<ProductDto?> FindProductByIdAsync(Guid productId)
    {
        var entity = context.Products.AsNoTracking().FirstOrDefault(x => x.Id == productId);

        return entity is null ? null : await MapToDto(entity);
    }

    public async ValueTask<ProductDto?> FindProductByArticleAsync(string article)
    {
        var entity = context.Products.AsNoTracking().FirstOrDefault(x => x.Article == article);

        return entity is null ? null : await MapToDto(entity);
    }

    public async ValueTask<ProductDto?> FindProductByNameAsync(string name)
    {
        var entity = context.Products.AsNoTracking().FirstOrDefault(x => x.Name == name);

        return entity is null ? null : await MapToDto(entity);
    }

    public async ValueTask CreateProductAsync(CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = MapToEntity(request);
            await context.Products.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async ValueTask UpdateProductAsync(UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = MapToEntity(request);
            context.Products.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async ValueTask<bool> DeleteProductAsync(DeleteProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = await context.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(x => request.ProductId == x.Id, cancellationToken);

            if (entity is null)
            {
                return false;
            }

            context.Products.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<TResponse?> FindAndMapAsync<TResponse>(ProductEntity entity) where TResponse : ProductEntity
    {
        var response = await context.Products
            .AsNoTracking()
            .OfType<TResponse>()
            .FirstOrDefaultAsync(x => x.Id == entity.Id);

        return response;
    }

    private async Task<ProductDto> MapToDto(ProductEntity entity)
    {
        var dto = entity?.ProductType switch
        {
            ProductTypes.Shoes => Mapper.ToProductDto((await FindAndMapAsync<ShoesEntity>(entity))!),
            ProductTypes.Clothing => Mapper.ToProductDto((await FindAndMapAsync<ClothingEntity>(entity))!),
            _ or ProductTypes.None => Mapper.ToProductDto(entity!),
        };

        return dto;
    }

    private ProductEntity MapToEntity(CreateProductRequest request)
    {
        var entity = request?.ProductType switch
        {
            ProductTypes.Shoes => Mapper.ToProductEntity(request),
            ProductTypes.Clothing => Mapper.ToProductEntity(request),
            _ or ProductTypes.None => Mapper.ToProductEntity(request!),
        };

        return entity;
    }

    private ProductEntity MapToEntity(UpdateProductRequest request)
    {
        var entity = request?.ProductType switch
        {
            ProductTypes.Shoes => Mapper.ToProductEntity(request),
            ProductTypes.Clothing => Mapper.ToProductEntity(request),
            _ or ProductTypes.None => Mapper.ToProductEntity(request!),
        };

        return entity;
    }
}