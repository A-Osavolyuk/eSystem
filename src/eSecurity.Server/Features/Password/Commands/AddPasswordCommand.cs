using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Password.Commands;

public record AddPasswordCommand(AddPasswordRequest Request) : IRequest<Result>;

public class AddPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        if (await _passwordManager.HasAsync(user, cancellationToken)) 
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPassword,
                Description = "User already has a password."
            });
        
        var result = await _passwordManager.AddAsync(user, request.Request.Password, cancellationToken);
        return result;
    }
}