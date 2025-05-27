using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangeEmailCommand(ConfirmEmailChangeRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangeEmailCommandHandler(
    IUserManager userManager,
    ISecurityManager securityManager)
    : IRequestHandler<ConfirmChangeEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecurityManager securityManager = securityManager;

    public async Task<Result> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.CurrentEmail, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.CurrentEmail}.");
        }

        var result = await securityManager.ChangeEmailAsync(user, request.Request.NewEmail, request.Request.CodeSet);

        return !result.Succeeded ? result : Result.Success("Your email address was successfully changed.");
    }
}