using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Core.Requests;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(IEmailManager emailManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var isTaken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
        if (isTaken)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "Email is already taken"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}