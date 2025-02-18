namespace eShop.Reviews.Api.Queries.Comments;

internal sealed record GetCommentsQuery(Guid ProductId) : IRequest<Result<GetCommentsResponse>>;

internal sealed class GetCommentsQueryHandler(
    AppDbContext context) : IRequestHandler<GetCommentsQuery, Result<GetCommentsResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<GetCommentsResponse>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var commentsList = await context.Comments
            .AsNoTracking()
            .Where(x => x.ProductId == request.ProductId)
            .ToListAsync(cancellationToken);

        var response = await commentsList
            .AsQueryable()
            .Select(x => Mapper.ToCommentDto(x))
            .ToListAsync(cancellationToken);

        return new(new GetCommentsResponse()
        {
            Comments = response,
            Message = "Successfully found comments",
        });
    }
}