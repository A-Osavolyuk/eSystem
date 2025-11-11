using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

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