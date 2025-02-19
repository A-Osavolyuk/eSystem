using eShop.Comments.Api.Commands.Comments;
using eShop.Comments.Api.Queries.Comments;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.Comments.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class CommentsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Get comments")]
    [EndpointDescription("Gets comments of a product")]
    [ProducesResponseType(200)]
    [HttpGet("get-comments/{productId:guid}")]
    public async ValueTask<ActionResult<Response>> GetCommentAsync(Guid productId)
    {
        var result = await sender.Send(new GetCommentsQuery(productId));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Creating a comment")]
    [EndpointDescription("Creates comments")]
    [ProducesResponseType(200)]
    [HttpPost("create-comment")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> CreateCommentAsync([FromBody] CreateCommentRequest request)
    {
        var result = await sender.Send(new CreateCommentCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Update comment")]
    [EndpointDescription("Updates the text of comment")]
    [ProducesResponseType(200)]
    [HttpPut("update-comment")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> UpdateCommentAsync([FromBody] UpdateCommentRequest request)
    {
        var result = await sender.Send(new UpdateCommentCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Delete comment")]
    [EndpointDescription("Deletes the comment")]
    [ProducesResponseType(200)]
    [HttpDelete("delete-comment")]
    public async ValueTask<ActionResult<Response>> DeleteCommentAsync([FromBody] DeleteCommentRequest request)
    {
        var result = await sender.Send(new DeleteCommentCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }
}