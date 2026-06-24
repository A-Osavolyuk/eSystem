using System.Text.Json.Serialization;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Password;

public sealed class ForgotPasswordCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public sealed class ForgotPasswordCommandHandler(
    IUserQueryService userQueryService,
    IPasswordManager passwordManager,
    ICodeManager codeManager,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
        var user = await _userQueryService.GetByEmailAsync(request.Email, cancellationToken);
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
            To = request.Email,
            Code = code
        };

        await _emailService.SendAsync(emailContext, cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}

public sealed class ForgotPasswordCommandValidator : IRequestValidator<ForgotPasswordCommand>
{
    public async ValueTask<Result> Validate(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}