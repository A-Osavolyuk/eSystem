using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangeEmailCommand(ConfirmEmailChangeRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangeEmailCommandHandler(IUserManager userManager) : IRequestHandler<ConfirmChangeEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var currentEmailCode = request.Request.CurrentEmailCode;
        var newEmailCode = request.Request.NewEmailCode;
        
        var result = await userManager.ChangeEmailAsync(user, request.Request.NewEmail, currentEmailCode, 
            newEmailCode, cancellationToken);

        return result;
    }
}