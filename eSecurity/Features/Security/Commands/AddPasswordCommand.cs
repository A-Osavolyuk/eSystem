using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Security.Commands;

public record AddPasswordCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class AddPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        
        if (user.HasPassword()) return Results.BadRequest("User already has a password.");
        
        var result = await passwordManager.AddAsync(user, request.Password, cancellationToken);
        return result;
    }
}