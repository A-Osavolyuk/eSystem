using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.Email;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Features.Email.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(IEmailManager emailManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var isTaken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
        if (isTaken)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "Email is already taken"
            });
        }

        return Results.Ok();
    }
}