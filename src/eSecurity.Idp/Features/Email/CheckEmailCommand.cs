using eSecurity.Core.Requests;
using eSecurity.Idp.Security.Identity.Email;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(IEmailQueryService emailQueryService) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        var isTaken = await _emailQueryService.ExistsAsync(request.Request.Email, cancellationToken);
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