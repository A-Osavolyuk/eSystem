using eSystem.Core.Common.Results;
using eSystem.Product.Api.Interfaces;
using eSystem.Product.Api.Mapping;

namespace eSystem.Product.Api.Features.Suppliers.Queries;

public record GetSuppliersQuery() : IRequest<Result>;

public class GetSuppliersQueryHandler(ISupplierManager supplierManager) : IRequestHandler<GetSuppliersQuery, Result>
{
    private readonly ISupplierManager supplierManager = supplierManager;

    public async Task<Result> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var entities = await supplierManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}