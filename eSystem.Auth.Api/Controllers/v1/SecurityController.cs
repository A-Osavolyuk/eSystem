using eSystem.Application.Common.Errors;
using eSystem.Auth.Api.Features.Security.Commands;
using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class SecurityController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Sign-in")]
    [EndpointDescription("Sign-in")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("sign-in")]
    [ValidationFilter]
    public async ValueTask<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var result = await sender.Send(new SignInCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Register")]
    [EndpointDescription("Register")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("register")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RegisterAsync([FromBody] RegistrationRequest request)
    {
        var result = await sender.Send(new RegisterCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Refresh token")]
    [EndpointDescription("Refresh token")]
    [ProducesResponseType(200)]
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var result = await sender.Send(new RefreshTokenCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Authorize")]
    [EndpointDescription("Authorize")]
    [ProducesResponseType(200)]
    [HttpPost("authorize")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> AuthorizeAsync([FromBody] AuthorizeRequest request)
    {
        var result = await sender.Send(new AuthorizeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Unauthorize")]
    [EndpointDescription("Unauthorize")]
    [ProducesResponseType(200)]
    [HttpPost("unauthorize")]
    [Authorize]
    public async ValueTask<IActionResult> UnauthorizeAsync([FromBody] UnauthorizeRequest request)
    {
        var result = await sender.Send(new UnauthorizeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Unlock account")]
    [EndpointDescription("Unlock account")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("account/unlock")]
    [ValidationFilter]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] UnlockAccountRequest request)
    {
        var result = await sender.Send(new UnlockAccountCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Recover account")]
    [EndpointDescription("Recover account")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("account/recover")]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await sender.Send(new RecoverAccountCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Check account")]
    [EndpointDescription("Check account")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("account/check")]
    [ValidationFilter]
    public async ValueTask<IActionResult> UnlockAsync([FromBody] CheckAccountRequest request)
    {
        var result = await sender.Send(new CheckAccountCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Add password")]
    [EndpointDescription("Add password")]
    [ProducesResponseType(200)]
    [HttpPost("password/add")]
    [ValidationFilter]
    public async ValueTask<IActionResult> AddPasswordAsync([FromBody] AddPasswordRequest request)
    {
        var result = await sender.Send(new AddPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [HttpPost("password/change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request)
    {
        var result = await sender.Send(new ChangePasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Check password")]
    [EndpointDescription("Check password")]
    [ProducesResponseType(200)]
    [HttpPost("password/check")]
    [ValidationFilter]
    public async ValueTask<IActionResult> CheckPasswordAsync(
        [FromBody] CheckPasswordRequest request)
    {
        var result = await sender.Send(new CheckPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Forgot password")]
    [EndpointDescription("Forgot password")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/forgot")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Reset password")]
    [EndpointDescription("Reset password")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var result = await sender.Send(new ResetPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Remove password")]
    [EndpointDescription("Remove password")]
    [ProducesResponseType(200)]
    [HttpPost("password/remove")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RemovePasswordAsync(RemovePasswordRequest request)
    {
        var result = await sender.Send(new RemovePasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change email")]
    [EndpointDescription("Change email")]
    [ProducesResponseType(200)]
    [HttpPost("email/change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangeEmailAsync(
        [FromBody] ChangeEmailRequest request)
    {
        var result = await sender.Send(new ChangeEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Add email")]
    [EndpointDescription("Add email")]
    [ProducesResponseType(200)]
    [HttpPost("email/add")]
    [ValidationFilter]
    public async ValueTask<IActionResult> AddEmailAsync(
        [FromBody] AddEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new AddEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify email")]
    [EndpointDescription("Verifies email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("email/verify")]
    [ValidationFilter]
    public async ValueTask<IActionResult> VerifyEmailAsync(
        [FromBody] VerifyEmailRequest request)
    {
        var result = await sender.Send(new VerifyEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Check email")]
    [EndpointDescription("Check email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("email/check")]
    [ValidationFilter]
    public async ValueTask<IActionResult> CheckEmailAsync(
        [FromBody] CheckEmailRequest request)
    {
        var result = await sender.Send(new CheckEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Reset email")]
    [EndpointDescription("Reset email")]
    [ProducesResponseType(200)]
    [HttpPost("email/reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ResetEmailAsync(
        [FromBody] ResetEmailRequest request)
    {
        var result = await sender.Send(new ResetEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Remove email")]
    [EndpointDescription("Remove email")]
    [ProducesResponseType(200)]
    [HttpPost("email/remove")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RemoveEmailAsync(
        [FromBody] RemoveEmailRequest request)
    {
        var result = await sender.Send(new RemoveEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Manager email")]
    [EndpointDescription("Manager email")]
    [ProducesResponseType(200)]
    [HttpPost("email/manage")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ManageEmailAsync(
        [FromBody] ManageEmailRequest request)
    {
        var result = await sender.Send(new ManageEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Add phone number")]
    [EndpointDescription("Add phone number change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/add")]
    [ValidationFilter]
    public async ValueTask<IActionResult> AddPhoneNumberAsync([FromBody] AddPhoneNumberRequest request)
    {
        var result = await sender.Send(new AddPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Remove phone number")]
    [EndpointDescription("Remove phone number change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/remove")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RemovePhoneNumberAsync([FromBody] RemovePhoneNumberRequest request)
    {
        var result = await sender.Send(new RemovePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify phone number")]
    [EndpointDescription("Verify phone number change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/verify")]
    [ValidationFilter]
    public async ValueTask<IActionResult> VerifyPhoneNumberAsync([FromBody] VerifyPhoneNumberRequest request)
    {
        var result = await sender.Send(new VerifyPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Check phone number")]
    [EndpointDescription("Check phone number")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/check")]
    [ValidationFilter]
    public async ValueTask<IActionResult> CheckPhoneNumberAsync([FromBody] CheckPhoneNumberRequest request)
    {
        var result = await sender.Send(new CheckPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change phone number")]
    [EndpointDescription("Change phone number")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ChangePhoneNumberAsync(
        [FromBody] ChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Reset phone number")]
    [EndpointDescription("Reset phone number")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ResetPhoneNumberAsync(
        [FromBody] ResetPhoneNumberRequest request)
    {
        var result = await sender.Send(new ResetPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}