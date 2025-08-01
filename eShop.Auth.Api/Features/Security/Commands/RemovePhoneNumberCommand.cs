using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemovePhoneNumberCommand(RemovePhoneNumberRequest Request) :  IRequest<Result>;

public class RemovePhoneNumberCommandHandler(
    IUserManager userManager) : IRequestHandler<RemovePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            return Results.BadRequest("Cannot remove phone number. Phone number is not provided.");
        }

        if (user.Providers.Any(x => x.Provider.Name == ProviderTypes.Sms && x.Subscribed))
        {
            return Results.BadRequest("Cannot remove phone number. First disable 2FA with SMS.");
        }
        
        var result = await userManager.RemovePhoneNumberAsync(user, cancellationToken);
        
        return result;
    }
}