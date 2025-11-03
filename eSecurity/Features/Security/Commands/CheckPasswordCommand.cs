using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;

namespace eSecurity.Features.Security.Commands;

public record CheckPasswordCommand(CheckPasswordRequest Request) : IRequest<Result>;

public class CheckPasswordCommandHandler(IUserManager userManager) : IRequestHandler<CheckPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if(user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = new CheckPasswordResponse()
        {
            HasPassword = user.HasPassword()
        };
        
        return Result.Success(response);
    }
}