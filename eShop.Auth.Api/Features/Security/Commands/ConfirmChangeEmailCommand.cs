namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangeEmailCommand(ConfirmChangeEmailRequest Request)
    : IRequest<Result<ConfirmChangeEmailResponse>>;

internal sealed class ConfirmChangeEmailCommandHandler(
    AppManager appManager)
    : IRequestHandler<ConfirmChangeEmailCommand, Result<ConfirmChangeEmailResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<ConfirmChangeEmailResponse>> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.CurrentEmail);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.CurrentEmail}."));
        }

        var result =
            await appManager.SecurityManager.ChangeEmailAsync(user, request.Request.NewEmail, request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot change email address of user with email {request.Request.CurrentEmail} " +
                $"due to server error: {result.Errors.First().Description}."));
        }

        return new(new ConfirmChangeEmailResponse()
        {
            Message = "Your email address was successfully changed."
        });
    }
}