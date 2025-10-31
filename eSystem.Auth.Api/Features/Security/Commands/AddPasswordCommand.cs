using eSystem.Auth.Api.Security.Authentication.Password;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record AddPasswordCommand(AddPasswordRequest Request) : IRequest<Result>;

public class AddPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        if (user.HasPassword()) return Results.BadRequest("User already has a password.");
        
        var result = await passwordManager.AddAsync(user, request.Request.Password, cancellationToken);
        return result;
    }
}