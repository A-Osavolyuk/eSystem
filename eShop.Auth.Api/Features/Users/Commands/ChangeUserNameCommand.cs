using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public record ChangeUserNameCommand(ChangeUserNameRequest Request) : IRequest<Result>;

public class ChangeUserNameCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<ChangeUserNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        if (identityOptions.Account.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUserNameTakenAsync(request.Request.UserName, cancellationToken);
        
            if (isUserNameTaken)
            {
                return Results.NotFound("Username is already taken");
            }
        }
        
        var result = await userManager.ChangeNameAsync(user, request.Request.UserName, cancellationToken);
        
        return result;
    }
}