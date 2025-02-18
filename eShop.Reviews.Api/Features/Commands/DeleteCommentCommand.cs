namespace eShop.Reviews.Api.Commands.Comments;

internal sealed record DeleteCommentCommand(DeleteCommentRequest Request) : IRequest<Result<DeleteCommentResponse>>;

internal sealed class DeleteCommentCommandHandler(
    AppDbContext context) : IRequestHandler<DeleteCommentCommand, Result<DeleteCommentResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<DeleteCommentResponse>> Handle(DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CommentId == request.Request.CommentId,
                cancellationToken: cancellationToken);

        if (comment is null)
        {
            return new(new NotFoundException($"Cannot find comment with id: {request.Request.CommentId}."));
        }

        context.Comments.Remove(comment);
        await context.SaveChangesAsync(cancellationToken);

        return new(new DeleteCommentResponse()
        {
            Message = "Comment successfully deleted.",
        });
    }
}