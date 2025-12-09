using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

public record CheckPasswordCommand(CheckPasswordRequest Request) : IRequest<Result>;

public class CheckPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<CheckPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result> Handle(CheckPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if(user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = new CheckPasswordResponse()
        {
            HasPassword = await _passwordManager.HasAsync(user, cancellationToken)
        };
        
        return Results.Ok(response);
    }
}