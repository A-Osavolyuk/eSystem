using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class VerifyPhoneNumberCommandHandler(IUserManager userManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number ${request.Request.PhoneNumber}");
        }

        var result = await userManager.ConfirmPhoneNumberAsync(user, request.Request.Code, cancellationToken);

        return result;
    }
}