namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangeEmailCommand(ConfirmChangeEmailRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangeEmailCommandHandler(
    AppManager appManager)
    : IRequestHandler<ConfirmChangeEmailCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.CurrentEmail);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.CurrentEmail}."
            });
        }

        var result =
            await appManager.SecurityManager.ChangeEmailAsync(user, request.Request.NewEmail, request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = $"Cannot change email address of user with email {request.Request.CurrentEmail} " +
                          $"due to server error: {result.Errors.First().Description}."
            });
        }

        return Result.Success("Your email address was successfully changed.");
    }
}