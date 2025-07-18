namespace eShop.Auth.Api.Features.Lockout.Query;

public record GetReasonsQuery() : IRequest<Result>;

public class GetReasonsQueryHandler(IReasonManager reasonManager) : IRequestHandler<GetReasonsQuery, Result>
{
    private readonly IReasonManager reasonManager = reasonManager;

    public async Task<Result> Handle(GetReasonsQuery request, CancellationToken cancellationToken)
    {
        var entities = await reasonManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        
        return Result.Success(result);
    }
}