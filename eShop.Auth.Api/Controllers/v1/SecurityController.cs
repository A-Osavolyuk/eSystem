using eShop.Auth.Api.Security.Schemes;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class SecurityController(ISender sender, ISignInManager signInManager) : ControllerBase
{
    private readonly ISender sender = sender;
    private readonly ISignInManager signInManager = signInManager;

    #region Get methods

    [EndpointSummary("External login")]
    [EndpointDescription("External login")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("oauth-login/{provider}")]
    public async ValueTask<ActionResult<Response>> ExternalLogin(string provider, string? returnUri = null)
    {
        var result = await sender.Send(new ExternalLoginQuery(provider, returnUri));

        return result.Match(
            s =>
            {
                var response = s.Value! as ExternalLoginResponse;
                return Challenge(response!.AuthenticationProperties, response.Provider);
            },
            ErrorHandler.Handle);
    }

    [EndpointSummary("Handle external login response")]
    [EndpointDescription("Handles external login response")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("handle-oauth-login")]
    public async ValueTask<ActionResult<Response>> HandleExternalLoginResponse(string? remoteError = null,
        string? returnUri = null)
    {
        var principal = await signInManager.AuthenticateAsync(HttpContext, ExternalAuthenticationDefaults.AuthenticationScheme);
        
        var result = await sender.Send(new HandleExternalLoginResponseQuery(principal, remoteError, returnUri));
        return result.Match(s => Redirect(Convert.ToString(s.Message!)!), ErrorHandler.Handle);
    }

    #endregion

    #region Post methods

    [EndpointSummary("Verify code")]
    [EndpointDescription("Verifies code")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("verify-code")]
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
    [HttpPost("resend-verification-code")]
    public async ValueTask<ActionResult<Response>> ResendVerificationCode(
        [FromBody] ResendEmailVerificationCodeRequest request)
    {
        var result = await sender.Send(new ResendEmailVerificationCodeCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
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

    [EndpointSummary("Reset password")]
    [EndpointDescription("Request password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async ValueTask<ActionResult<Response>> ResetPasswordRequest(ResetPasswordRequest request)
    {
        var result = await sender.Send(new RequestResetPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify email")]
    [EndpointDescription("Verifies email")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async ValueTask<ActionResult<Response>> ConfirmEmail(
        [FromBody] VerifyEmailRequest request)
    {
        var result = await sender.Send(new VerifyEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Confirm change email")]
    [EndpointDescription("Confirms an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("confirm-email")]
    public async ValueTask<ActionResult<Response>> ConfirmChangeEmail(
        [FromBody] ConfirmChangeEmailRequest request)
    {
        var result = await sender.Send(new ConfirmChangeEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Confirm change phone number")]
    [EndpointDescription("Confirm a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPost("confirm-phone-number")]
    public async ValueTask<ActionResult<Response>> ConfirmChangePhoneNumber(
        [FromBody] ConfirmChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
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

    #endregion

    #region Put methods

    [EndpointSummary("Confirm password")]
    [EndpointDescription("Confirm password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPut("confirm-password")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmResetPassword(
        [FromBody] ConfirmResetPasswordRequest confirmPasswordResetRequest)
    {
        var result = await sender.Send(new ConfirmResetPasswordCommand(confirmPasswordResetRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change email")]
    [EndpointDescription("Request an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPut("request-change-email")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangeEmail(
        [FromBody] ChangeEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new ChangeEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Request change phone number")]
    [EndpointDescription("Request a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPut("request-change-phone-number")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangePhoneNumber(
        [FromBody] ChangePhoneNumberRequest changePhoneNumberRequest)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(changePhoneNumberRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "UpdateAccountPolicy")]
    [HttpPut("change-password")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePassword(
        [FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new ChangePasswordCommand(changePasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }

    #endregion
}