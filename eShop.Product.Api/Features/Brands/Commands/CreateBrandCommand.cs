using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Brand;

namespace eShop.Product.Api.Features.Brands.Commands;

internal sealed record CreateBrandCommand(CreateBrandRequest Request) : IRequest<Result>;

internal sealed class CreateBrandCommandHandle(
    AppDbContext context) : IRequestHandler<CreateBrandCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var entity = Mapper.Map(request.Request);
        await context.Brands.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Successfully created brand");
    }
}