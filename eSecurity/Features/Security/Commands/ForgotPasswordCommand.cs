using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;

namespace eSecurity.Features.Security.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler(
    IUserManager userManager) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with email {request.Request.Email}.");

        if (!user.HasPassword()) return Results.BadRequest("Cannot reset password, password was not provided.");

        var response = new ForgotPasswordResponse() { UserId = user.Id };
        return Result.Success(response);
    }
}