using eShop.Auth.Api.Security.Identity.Options;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public record ChangeUsernameCommand(ChangeUsernameRequest Request) : IRequest<Result>;

public class ChangeUsernameCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangeUsernameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        if (user.Username == request.Request.Username) 
            return Results.BadRequest("New username must be different than the current username");
        
        if (options.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUsernameTakenAsync(request.Request.Username, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }
        
        var result = await userManager.ChangeUsernameAsync(user, request.Request.Username, cancellationToken);
        return result;
    }
}