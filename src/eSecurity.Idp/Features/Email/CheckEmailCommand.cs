using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Identity.Email;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email;

public sealed class CheckEmailCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}

public class CheckEmailCommandHandler(IEmailQueryService emailQueryService) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        if (await _emailQueryService.ExistsAsync(request.Email, cancellationToken))
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