using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class SecurityController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Login")]
    [EndpointDescription("Login")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("login")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> Login([FromBody] LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Register")]
    [EndpointDescription("Register")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("register")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> Register([FromBody] RegistrationRequest request)
    {
        var result = await sender.Send(new RegisterCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Refresh token")]
    [EndpointDescription("Refresh token")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("refresh-token")]
    public async ValueTask<ActionResult<Response>> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var result = await sender.Send(new RefreshTokenCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Recover account")]
    [EndpointDescription("Recover account")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("account/recover")]
    public async ValueTask<ActionResult<Response>> RecoverAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await sender.Send(new RecoverAccountCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("password/change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePassword(
        [FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new ChangePasswordCommand(changePasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Reset password")]
    [EndpointDescription("Request password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/forgot")]
    public async ValueTask<ActionResult<Response>> ForgotPasswordRequest(ForgotPasswordRequest request)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm password")]
    [EndpointDescription("Confirm password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmResetPassword(
        [FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var result = await sender.Send(new ResetPasswordCommand(resetPasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change email")]
    [EndpointDescription("Request an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("email/request-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangeEmail(
        [FromBody] ChangeEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new ChangeEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change email")]
    [EndpointDescription("Confirms an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("email/confirm-change")]
    public async ValueTask<ActionResult<Response>> ConfirmChangeEmail(
        [FromBody] ConfirmEmailChangeRequest request)
    {
        var result = await sender.Send(new ConfirmChangeEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify email")]
    [EndpointDescription("Verifies email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("email/verify")]
    public async ValueTask<ActionResult<Response>> ConfirmEmail(
        [FromBody] VerifyEmailRequest request)
    {
        var result = await sender.Send(new VerifyEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Add phone number")]
    [EndpointDescription("Add phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("phone-number/add")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> AddPhoneNumberAsync([FromBody] AddPhoneNumberRequest request)
    {
        var result = await sender.Send(new AddPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify phone number")]
    [EndpointDescription("Verify phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("phone-number/verify")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> VerifyPhoneNumberAsync([FromBody] VerifyPhoneNumberRequest request)
    {
        var result = await sender.Send(new VerifyPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change phone number")]
    [EndpointDescription("Request a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("phone-number/request-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangePhoneNumber(
        [FromBody] ChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change phone number")]
    [EndpointDescription("Confirm a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("phone-number/confirm-change")]
    public async ValueTask<ActionResult<Response>> ConfirmChangePhoneNumber(
        [FromBody] ConfirmChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify code")]
    [EndpointDescription("Verifies code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("code/verify")]
    public async ValueTask<ActionResult<Response>> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await sender.Send(new VerifyCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Resend verification code")]
    [EndpointDescription("Resends verification code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("code/resend")]
    public async ValueTask<ActionResult<Response>> ResendVerificationCode(
        [FromBody] ResendEmailVerificationCodeRequest request)
    {
        var result = await sender.Send(new ResendEmailVerificationCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}