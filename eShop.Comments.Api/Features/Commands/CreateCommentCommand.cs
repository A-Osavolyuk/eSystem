namespace eShop.Comments.Api.Features.Commands;

internal sealed record CreateCommentCommand(CreateCommentRequest Request) : IRequest<Result>;

internal sealed class CreateCommentCommandHandler(
    AppDbContext context,
    ILogger<CreateCommentCommandHandler> logger) : IRequestHandler<CreateCommentCommand, Result>
{
    private readonly AppDbContext context = context;
    private readonly ILogger<CreateCommentCommandHandler> logger = logger;

    public async Task<Result> Handle(CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = Mapper.Map(request.Request);
        await context.Comments.AddAsync(comment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment was successfully created.");
    }
}