using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ProfileController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Get personal data")]
    [EndpointDescription("Gets personal data")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpGet("get-personal-data/{email}")]
    public async ValueTask<ActionResult<Response>> GetPersonalData(string email)
    {
        var result = await sender.Send(new GetPersonalDataQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get phone number")]
    [EndpointDescription("Gets a phone number")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpGet("get-phone-number/{email}")]
    public async ValueTask<ActionResult<Response>> GetPhoneNumber(string email)
    {
        var result = await sender.Send(new GetPhoneNumberQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change username")]
    [EndpointDescription("Changes username")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPatch("change-user-name")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangeUserName(
        [FromBody] ChangeUserNameRequest changeUserNameRequest)
    {
        var result = await sender.Send(new ChangeUserNameCommand(changeUserNameRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message)
                .WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change personal data")]
    [EndpointDescription("Changes personal data")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPut("change-personal-data")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePersonalData(
        [FromBody] ChangePersonalDataRequest changePersonalDataRequest)
    {
        var result = await sender.Send(new ChangePersonalDataCommand(changePersonalDataRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded()
                .WithMessage("Personal data was successfully changed.")
                .WithResult(s).Build()),
            ErrorHandler.Handle);
    }
}