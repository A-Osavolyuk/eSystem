using eSecurity.Common.Responses;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Security.Commands;

public record CheckPasswordCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class CheckPasswordCommandHandler(IUserManager userManager) : IRequestHandler<CheckPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if(user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new CheckPasswordResponse()
        {
            HasPassword = user.HasPassword()
        };
        
        return Result.Success(response);
    }
}