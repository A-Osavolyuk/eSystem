using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Users.Commands;

public record ChangeUsernameCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class ChangeUsernameCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangeUsernameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        if (user.Username == request.Username) 
            return Results.BadRequest("New username must be different than the current username");
        
        if (options.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUsernameTakenAsync(request.Username, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }
        
        var result = await userManager.ChangeUsernameAsync(user, request.Username, cancellationToken);
        return result;
    }
}