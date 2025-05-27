using eShop.Auth.Api.Features.Users.Commands;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Create user account")]
    [EndpointDescription("Create a user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "CreateUserPolicy")]
    [HttpPost]
    public async ValueTask<ActionResult<Response>> CreateUserAccount([FromBody] CreateUserAccountRequest request)
    {
        var result = await sender.Send(new CreateUserAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Delete user account")]
    [EndpointDescription("Deletes user account")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "DeleteUsersPolicy")]
    [HttpDelete]
    public async ValueTask<ActionResult<Response>> DeleteUserAccountAsync([FromBody] DeleteUserAccountRequest request)
    {
        var result = await sender.Send(new DeleteUserAccountCommand(request));
        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}