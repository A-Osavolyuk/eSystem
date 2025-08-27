using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckPhoneNumberCommand(CheckPhoneNumberRequest Request) : IRequest<Result>;

public class CheckPhoneNumberCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<CheckPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(CheckPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        if (identityOptions.Account.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }
        
        return Result.Success();
    }
}