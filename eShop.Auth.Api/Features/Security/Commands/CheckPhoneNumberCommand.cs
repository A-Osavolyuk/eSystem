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
        CheckPhoneNumberResponse? response;
        
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);
        if (!isTaken)
        {
            response = new CheckPhoneNumberResponse { IsTaken = false };
            return Result.Success(response);
        }
        
        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(
            x => x.PhoneNumber == request.Request.PhoneNumber);
        
        if (userPhoneNumber is null)
        {
            response = new CheckPhoneNumberResponse { IsTaken = true };
            return Result.Success(response);
        }
        
        response = new CheckPhoneNumberResponse
        {
            IsOwn = true,
            IsTaken = true
        };
        
        return Result.Success(response);
    }
}