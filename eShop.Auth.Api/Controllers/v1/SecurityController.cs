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
    public async ValueTask<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request, HttpContext));

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
        var result = await sender.Send(new RegisterCommand(request, HttpContext));

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
    
    [EndpointSummary("Trust device")]
    [EndpointDescription("Trust device")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("device/trust")]
    [ValidationFilter]
    public async ValueTask<IActionResult> TrustDeviceAsync([FromBody] TrustDeviceRequest request)
    {
        var result = await sender.Send(new TrustDeviceCommand(request));

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
    
    [EndpointSummary("Add password")]
    [EndpointDescription("Add password")]
    [ProducesResponseType(200)]
    [HttpPost("password/add")]
    [ValidationFilter]
    public async ValueTask<IActionResult> AddPasswordAsync([FromBody] AddPasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new AddPasswordCommand(changePasswordRequest));

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
        [FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new ChangePasswordCommand(changePasswordRequest));

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
    
    [EndpointSummary("Confirm forgot password")]
    [EndpointDescription("Confirm forgot password")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("password/confirm-forgot")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmForgotPasswordAsync(
        [FromBody] ConfirmForgotPasswordRequest confirmForgotPasswordRequest)
    {
        var result = await sender.Send(new ConfirmForgotPasswordCommand(confirmForgotPasswordRequest));

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

    [EndpointSummary("Request change email")]
    [EndpointDescription("Request an email change")]
    [ProducesResponseType(200)]
    [HttpPost("email/request-change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RequestChangeEmailAsync(
        [FromBody] ChangeEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new ChangeEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify current email on change")]
    [EndpointDescription("Verify current email on change")]
    [ProducesResponseType(200)]
    [HttpPost("email/verify-current")]
    [ValidationFilter]
    public async ValueTask<IActionResult> VerifyCurrentEmailAsync(
        [FromBody] VerifyCurrentEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new VerifyCurrentEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change email")]
    [EndpointDescription("Confirms an email change")]
    [ProducesResponseType(200)]
    [HttpPost("email/confirm-change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmChangeEmailAsync(
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
    public async ValueTask<IActionResult> VerifyEmailAsync(
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
    [HttpPost("email/request-reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ResetEmailAsync(
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
    [HttpPost("email/confirm-reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmEmailResetAsync(
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
    public async ValueTask<IActionResult> AddRecoveryEmailAsync(
        [FromBody] AddRecoveryEmailRequest request)
    {
        var result = await sender.Send(new AddRecoveryEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Remove recovery email")]
    [EndpointDescription("Remove recovery email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("recovery-email/remove")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RemoveRecoveryEmailAsync(
        [FromBody] RemoveRecoveryEmailRequest request)
    {
        var result = await sender.Send(new RemoveRecoveryEmailCommand(request));

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
    public async ValueTask<IActionResult> VerifyRecoveryEmailAsync(
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

    [EndpointSummary("Request change phone number")]
    [EndpointDescription("Request a phone number change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/request-change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> RequestChangePhoneNumberAsync(
        [FromBody] ChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify current phone number on change")]
    [EndpointDescription("Verify current phone number on change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/verify-current")]
    [ValidationFilter]
    public async ValueTask<IActionResult> VerifyCurrentPhoneNumberAsync(
        [FromBody] VerifyCurrentPhoneNumberRequest request)
    {
        var result = await sender.Send(new VerifyCurrentPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Confirm change phone number")]
    [EndpointDescription("Confirm a phone number change")]
    [ProducesResponseType(200)]
    [HttpPost("phone-number/confirm-change")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmChangePhoneNumberAsync(
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
    [HttpPost("phone-number/request-reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ResetPhoneNumberAsync(
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
    [HttpPost("phone-number/confirm-reset")]
    [ValidationFilter]
    public async ValueTask<IActionResult> ConfirmPhoneNumberResetAsync(
        [FromBody] ConfirmResetPhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmResetPhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Resend code")]
    [EndpointDescription("Resends code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("code/resend")]
    public async ValueTask<IActionResult> ResendCodeAsync(
        [FromBody] ResendCodeRequest request)
    {
        var result = await sender.Send(new ResendCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}