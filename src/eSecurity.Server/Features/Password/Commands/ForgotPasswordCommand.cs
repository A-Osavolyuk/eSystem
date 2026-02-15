using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IMessageService _messageService = messageService;

    public async Task<Result> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPassword,
                Description = "Password was not provided."
            });
        }

        var code = await _codeManager.GenerateAsync(user, SenderType.Email, 
            ActionType.Reset, PurposeType.Password, cancellationToken);
        
        var message = new CodeEmailMessage()
        {
            Credentials = new Dictionary<string, string>
            {
                { "To", request.Request.Email },
                { "Subject", "Forgot password" },
            },
            Payload = new()
            {
                { "Code", code }
            }
        };
        
        await _messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        var response = new ForgotPasswordResponse { UserId = user.Id };
        return Results.Ok(response);
    }
}