using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request) : IRequest<Result>;

public sealed class VerifyPhoneNumberCommandHandler(IUserManager userManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID ${request.Request.Id}");
        }

        var result = await userManager.ConfirmPhoneNumberAsync(user, request.Request.Code, cancellationToken);

        return result;
    }
}