using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = new CheckPhoneNumberResponse()
        {
            IsTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken)
        };
        
        return Result.Success(response);
    }
}