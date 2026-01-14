using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Email.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var isTaken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
        if (isTaken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.EmailTaken,
                Description = "Email is already taken"
            });
        }

        return Results.Ok();
    }
}