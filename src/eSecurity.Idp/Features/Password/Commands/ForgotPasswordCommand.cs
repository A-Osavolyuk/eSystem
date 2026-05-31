using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Password.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    ICodeManager codeManager,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPassword,
                Description = "Password was not provided."
            });
        }

        var codeResult = await _codeManager.CreateAsync(user, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!codeResult.TryGetValue(out var code))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var emailContext = new CodeVerificationEmailContext()
        {
            Subject = "Forgot password",
            To = request.Request.Email,
            Code = code
        };

        await _emailService.SendAsync(emailContext, cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}