namespace eShop.Reviews.Api.Commands.Comments;

internal sealed record UpdateCommentCommand(UpdateCommentRequest Request) : IRequest<Result<UpdateCommentResponse>>;

internal sealed class UpdateCommentCommandHandler(
    AppDbContext context) : IRequestHandler<UpdateCommentCommand, Result<UpdateCommentResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<UpdateCommentResponse>> Handle(UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CommentId == request.Request.CommentId, cancellationToken);

        if (comment is null)
        {
            return new(new NotFoundException($"Cannot find comment with id: {request.Request.CommentId}."));
        }

        var newComment = Mapper.ToCommentEntity(request.Request) with { UpdatedAt = DateTime.UtcNow };
        context.Comments.Update(newComment);
        await context.SaveChangesAsync(cancellationToken);

        return new Result<UpdateCommentResponse>(new UpdateCommentResponse()
        {
            Message = "Comment was successfully updated.",
        });
    }
}