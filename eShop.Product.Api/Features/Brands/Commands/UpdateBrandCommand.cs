using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Brand;

namespace eShop.Product.Api.Features.Brands.Commands;

internal sealed record UpdateBrandCommand(UpdateBrandRequest Request) : IRequest<Result>;

internal sealed class UpdateBrandCommandHandler(
    AppDbContext context) : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        if (!await context.Brands.AsNoTracking().AnyAsync(x => x.Id == request.Request.Id, cancellationToken))
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find brand with ID {request.Request.Id}"
            });
        }

        var entity = Mapper.Map(request.Request);
        context.Brands.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Brand was successfully updated");
    }
}