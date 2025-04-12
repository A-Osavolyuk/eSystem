using eShop.Domain.Common.Api;
using eShop.Domain.Enums;
using eShop.Domain.Requests.Api.Comments;

namespace eShop.Comments.Api.Features.Commands;

internal sealed record UpdateCommentCommand(UpdateCommentRequest Request) : IRequest<Result>;

internal sealed class UpdateCommentCommandHandler(
    AppDbContext context) : IRequestHandler<UpdateCommentCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Request.CommentId, cancellationToken);

        if (comment is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find comment with id: {request.Request.CommentId}."
            });
        }

        var newComment = Mapper.Map(request.Request) with { UpdateDate = DateTime.UtcNow };
        context.Comments.Update(newComment);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment was successfully updated.");
    }
}