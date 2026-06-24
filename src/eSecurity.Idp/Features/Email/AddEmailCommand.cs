using System.Text.Json.Serialization;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Email;

public sealed class AddEmailCommand : IRequest<Result>
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public sealed class AddEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IEmailPolicy emailPolicy,
    IOptions<AccountOptions> options) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IEmailPolicy _emailPolicy = emailPolicy;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var userEmails = await _emailQueryService.ListByUserAsync(user.Id, cancellationToken);
        var canAddResult = _emailPolicy.CanAdd(userEmails, EmailType.Secondary);
        if (!canAddResult.Succeeded) return canAddResult;

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

        var email = request.Email;
        return await _emailCommandService.AddAsync(user.Id, email, EmailType.Secondary, cancellationToken);
    }
}

public sealed class AddEmailCommandValidator : IRequestValidator<AddEmailCommand>
{
    public async ValueTask<Result> Validate(AddEmailCommand request, CancellationToken cancellationToken)
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