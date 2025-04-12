using eShop.Application;
using eShop.Domain.Common.API;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

internal sealed class VerifyEmailCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    CartClient client) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly CartClient client = client;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var confirmResult = await appManager.SecurityManager.VerifyEmailAsync(user, request.Request.Code);

        if (!confirmResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error.",
                Details = $"Cannot confirm email address of user with email {request.Request.Email} " +
                          $"due to server error: {confirmResult.Errors.First().Description}."
            });
        }

        await messageService.SendMessageAsync("email-verified", new EmailVerifiedMessage()
        {
            To = request.Request.Email,
            Subject = "Email verified",
            UserName = user.UserName!
        });

        var response = await client.InitiateUserAsync(new InitiateUserRequest() { UserId = user.Id.ToString() });

        if (!response.IsSucceeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = response.Message
            });
        }

        return Result.Success("Your email address was successfully confirmed.");
    }
}