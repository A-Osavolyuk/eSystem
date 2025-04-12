using eShop.Domain.Requests.Api.Comments;
using eShop.Domain.Responses.Api.Comments;

namespace eShop.Comments.Api.Receivers;

public class ProductDeletedReceiver(
    ISender sender,
    ILogger<ProductDeletedReceiver> logger,
    AppDbContext dbContext)
    : IConsumer<DeleteCommentsRequest>
{
    private readonly ISender sender = sender;
    private readonly ILogger<ProductDeletedReceiver> logger = logger;
    private readonly AppDbContext dbContext = dbContext;

    public async Task Consume(ConsumeContext<DeleteCommentsRequest> context)
    {
        logger.LogInformation("Got message with command to delete comments with product ID: {id}.",
            context.Message.ProductId);

        var comments = await dbContext.Comments
            .AsNoTracking()
            .Where(c => c.ProductId == context.Message.ProductId)
            .ToListAsync();

        if (comments.Any())
        {
            dbContext.Comments.RemoveRange(comments);
            await dbContext.SaveChangesAsync();
        }

        await context.RespondAsync<DeleteCommentResponse>(new DeleteCommentResponse()
        {
            Message = "Comments were successfully deleted.",
        });

        logger.LogInformation($"Response was successfully sent.");
    }
}