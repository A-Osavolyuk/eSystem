namespace eShop.Auth.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class AuthController(SignInManager<AppUser> signInManager, ISender sender) : ControllerBase
{
    private readonly SignInManager<AppUser> signInManager = signInManager;
    private readonly ISender sender = sender;

    #region Get methods

    [EndpointSummary("Get 2FA state")]
    [EndpointDescription("Get 2FA state")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpGet("get-2fa-state/{email}")]
    public async ValueTask<ActionResult<Response>> GetTwoFactorAuthenticationState(string email)
    {
        var result = await sender.Send(new GetTwoFactorAuthenticationStateQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithMessage(s.State.Enabled
                    ? "Two factor authentication state is enabled."
                    : "Two factor authentication state is disabled.")
                .WithResult(s)
                .Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("External login")]
    [EndpointDescription("External login")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("external-login/{provider}")]
    public async ValueTask<ActionResult<Response>> ExternalLogin(string provider, string? returnUri = null)
    {
        var result = await sender.Send(new ExternalLoginQuery(provider, returnUri));

        return result.Match(
            s => Challenge(s.AuthenticationProperties, s.Provider),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Handle external login response")]
    [EndpointDescription("Handles external login response")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("handle-external-login-response")]
    public async ValueTask<ActionResult<Response>> HandleExternalLoginResponse(string? remoteError = null,
        string? returnUri = null)
    {
        var info = await signInManager.GetExternalLoginInfoAsync();

        var result = await sender.Send(new HandleExternalLoginResponseQuery(info!, remoteError, returnUri));

        return result.Match(
            Redirect,
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Get external login providers")]
    [EndpointDescription("Gets external login providers")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpGet("get-external-providers")]
    public async ValueTask<ActionResult<Response>> GetExternalProvidersList()
    {
        var result = await sender.Send(new GetExternalProvidersQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ExceptionHandler.HandleException);
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
            succ => Ok(new ResponseBuilder().Succeeded().WithMessage(succ.Message).Build()),
            ExceptionHandler.HandleException);
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
            succ => Ok(new ResponseBuilder().Succeeded().WithMessage(succ.Message).Build()),
            ExceptionHandler.HandleException);
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
            succ => Ok(new ResponseBuilder().Succeeded().WithMessage(succ.Message).Build()),
            ExceptionHandler.HandleException);
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
            succ => Ok(new ResponseBuilder().Succeeded().WithResult(succ).WithMessage(succ.Message).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Request reset password")]
    [EndpointDescription("Requests reset password")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("request-reset-password")]
    public async ValueTask<ActionResult<Response>> ResetPasswordRequest(ResetPasswordRequest request)
    {
        var result = await sender.Send(new RequestResetPasswordCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ExceptionHandler.HandleException);
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
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Change 2FA state")]
    [EndpointDescription("Changes 2FA state")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPost("change-2fa-state")]
    public async ValueTask<ActionResult<Response>> ChangeTwoFactorAuthentication(
        [FromBody] ChangeTwoFactorAuthenticationRequest request)
    {
        var result = await sender.Send(new ChangeTwoFactorAuthenticationStateCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Login 2FA")]
    [EndpointDescription("Login with 2FA")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPost("2fa-login")]
    public async ValueTask<ActionResult<Response>> LoginWithTwoFactorAuthenticationCode(
        [FromBody] TwoFactorAuthenticationLoginRequest request)
    {
        var result =
            await sender.Send(new TwoFactorAuthenticationLoginCommand(request));

        return result.Match(
            succ => Ok(new ResponseBuilder().Succeeded().WithResult(succ).WithMessage(succ.Message).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Confirm change email")]
    [EndpointDescription("Confirms an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPost("confirm-change-email")]
    public async ValueTask<ActionResult<Response>> ConfirmChangeEmail(
        [FromBody] ConfirmChangeEmailRequest request)
    {
        var result = await sender.Send(new ConfirmChangeEmailCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Confirm change phone number")]
    [EndpointDescription("Confirm a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPost("confirm-change-phone-number")]
    public async ValueTask<ActionResult<Response>> ConfirmChangePhoneNumber(
        [FromBody] ConfirmChangePhoneNumberRequest request)
    {
        var result = await sender.Send(new ConfirmChangePhoneNumberCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    #endregion

    #region Put methods

    [EndpointSummary("Confirm reset password")]
    [EndpointDescription("Confirm password reset")]
    [ProducesResponseType(200)]
    [AllowAnonymous]
    [HttpPut("confirm-reset-password")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ConfirmResetPassword(
        [FromBody] ConfirmResetPasswordRequest confirmPasswordResetRequest)
    {
        var result = await sender.Send(new ConfirmResetPasswordCommand(confirmPasswordResetRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Request change email")]
    [EndpointDescription("Request an email change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPut("request-change-email")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangeEmail(
        [FromBody] ChangeEmailRequest changeEmailRequest)
    {
        var result = await sender.Send(new ChangeEmailCommand(changeEmailRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Request change phone number")]
    [EndpointDescription("Request a phone number change")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPut("request-change-phone-number")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> RequestChangePhoneNumber(
        [FromBody] ChangePhoneNumberRequest changePhoneNumberRequest)
    {
        var result = await sender.Send(new ChangePhoneNumberCommand(changePhoneNumberRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    [EndpointSummary("Change password")]
    [EndpointDescription("Change password")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPut("change-password")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> ChangePassword(
        [FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var result = await sender.Send(new ChangePasswordCommand(changePasswordRequest));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s).Build()),
            ExceptionHandler.HandleException);
    }

    #endregion
}