using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public record ChangeUserNameCommand(ChangeUserNameRequest Request) : IRequest<Result>;

public class ChangeUserNameCommandHandler(IUserManager userManager) : IRequestHandler<ChangeUserNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.Id}");
        }
        
        var result = await userManager.ChangeUsernameAsync(user, request.Request.UserName, cancellationToken);
        
        return result;
    }
}