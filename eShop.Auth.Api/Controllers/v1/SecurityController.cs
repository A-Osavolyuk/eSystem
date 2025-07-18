using eShop.Domain.Requests;
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
    public async ValueTask<ActionResult<Response>> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request));

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
    public async ValueTask<ActionResult<Response>> RegisterAsync([FromBody] RegistrationRequest request)
    {
        var result = await sender.Send(new RegisterCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
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
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Rollback")]
    [EndpointDescription("Rollback")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("rollback")]
    public async ValueTask<ActionResult<Response>> RollbackAsync([FromBody] RollbackRequest request)
    {
        var result = await sender.Send(new RollbackCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Recover account")]
    [EndpointDescription("Recover account")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("account/recover")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RecoverAsync([FromBody] RecoverAccountRequest request)
    {
        var result = await sender.Send(new RecoverAccountCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("password/change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new ChangePasswordCommand(changePasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Reset password")]
    [EndpointDescription("Request password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/forgot")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ForgotPasswordRequestAsync(ForgotPasswordRequest request)
    {
        var result = await sender.Send(new ForgotPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm password")]
    [EndpointDescription("Confirm password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmResetPasswordAsync(
        [FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var result = await sender.Send(new ResetPasswordCommand(resetPasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change email")]
    [EndpointDescription("Request an email change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("email/request-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangeEmailAsync(
        [FromBody] ChangeEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new ChangeEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change email")]
    [EndpointDescription("Confirms an email change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("email/confirm-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmChangeEmailAsync(
        [FromBody] ConfirmChangeEmailRequest request)
    {
        var result = await sender.Send(new ConfirmChangeEmailCommand(request));

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
    public async ValueTask<ActionResult<Response>> VerifyEmailAsync(
        [FromBody] VerifyEmailRequest request)
    {
        var result = await sender.Send(new VerifyEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Reset email")]
    [EndpointDescription("Reset email")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("email/request-reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ResetEmailAsync(
        [FromBody] ResetEmailRequest request)
    {
        var result = await sender.Send(new ResetEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm email reset")]
    [EndpointDescription("Confirm email reset")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("email/confirm-reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmEmailResetAsync(
        [FromBody] ConfirmResetEmailRequest request)
    {
        var result = await sender.Send(new ConfirmResetEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Add recovery email")]
    [EndpointDescription("Add recovery email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("recovery-email/add")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> AddRecoveryEmailAsync(
        [FromBody] AddRecoveryEmailRequest request)
    {
        var result = await sender.Send(new AddRecoveryEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify recovery email")]
    [EndpointDescription("Verify recovery email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("recovery-email/verify")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> VerifyRecoveryEmailAsync(
        [FromBody] VerifyRecoveryEmailRequest request)
    {
        var result = await sender.Send(new VerifyRecoveryEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Add phone number")]
    [EndpointDescription("Add phone number change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/add")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> AddPhoneNumberAsync([FromBody] AddPhoneNumberRequest request)
    {
        var result = await sender.Send(new AddPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify phone number")]
    [EndpointDescription("Verify phone number change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/verify")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> VerifyPhoneNumberAsync([FromBody] VerifyPhoneNumberRequest request)
    {
        var result = await sender.Send(new VerifyPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change phone number")]
    [EndpointDescription("Request a phone number change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/request-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangePhoneNumberAsync(
        [FromBody] ChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change phone number")]
    [EndpointDescription("Confirm a phone number change")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/confirm-change")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmChangePhoneNumberAsync(
        [FromBody] ConfirmChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Reset phone number")]
    [EndpointDescription("Reset phone number")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/request-reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ResetPhoneNumberAsync(
        [FromBody] ResetPhoneNumberRequest request)
    {
        var result = await sender.Send(new ResetPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm phone number reset")]
    [EndpointDescription("Confirm phone number reset")]
    [ProducesResponseType(200)]
    [Authorize]
    [HttpPost("phone-number/confirm-reset")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmPhoneNumberResetAsync(
        [FromBody] ConfirmResetPhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmResetPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify code")]
    [EndpointDescription("Verifies code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("code/verify")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await sender.Send(new VerifyCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Resend code")]
    [EndpointDescription("Resends code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("code/resend")]
    public async ValueTask<ActionResult<Response>> ResendCodeAsync(
        [FromBody] ResendCodeRequest request)
    {
        var result = await sender.Send(new ResendCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}