namespace eShop.Comments.Api.Features.Queries;

internal sealed record GetCommentsQuery(Guid ProductId) : IRequest<Result>;

internal sealed class GetCommentsQueryHandler(
    AppDbContext context) : IRequestHandler<GetCommentsQuery, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var commentsList = await context.Comments
            .AsNoTracking()
            .Where(x => x.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        var response = await commentsList
            .AsQueryable()
            .Select(x => Mapper.Map(x))
            .ToListAsync(cancellationToken);

        return Result.Success(new GetCommentsResponse()
        {
            Comments = response,
            Message = "Successfully found comments",
        });
    }
}