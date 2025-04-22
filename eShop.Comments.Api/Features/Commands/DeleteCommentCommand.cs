using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Comments;
using Results = eShop.Domain.Common.API.Results;

namespace eShop.Comments.Api.Features.Commands;

internal sealed record DeleteCommentCommand(DeleteCommentRequest Request) : IRequest<Result>;

internal sealed class DeleteCommentCommandHandler(
    AppDbContext context) : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Request.CommentId,
                cancellationToken: cancellationToken);

        if (comment is null)
        {
            return Results.NotFound($"Cannot find comment with id: {request.Request.CommentId}.");
        }

        context.Comments.Remove(comment);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment successfully deleted.");
    }
}