using System.Text.Json.Serialization;
using eSecurity.Idp.Features.Email.Change;
using eSecurity.Idp.Security.Identity.Email;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Email;

public sealed class CheckEmailCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public sealed class CheckEmailCommandHandler(IEmailQueryService emailQueryService) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        
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

public sealed class CheckEmailCommandValidator : IRequestValidator<CheckEmailCommand>
{
    public async ValueTask<Result> Validate(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'email' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}