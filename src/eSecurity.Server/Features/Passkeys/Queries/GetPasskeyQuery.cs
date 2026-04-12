using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Server.Features.Passkeys.Queries;

public record GetPasskeyQuery(Guid Id) : IRequest<Result>;

public class GetPasskeyQueryHandler(IPasskeyManager passkeyManager) : IRequestHandler<GetPasskeyQuery, Result>
{
    private readonly IPasskeyManager _passkeyManager = passkeyManager;

    public async Task<Result> Handle(GetPasskeyQuery request, CancellationToken cancellationToken)
    {
        var passkey = await _passkeyManager.FindByIdAsync(request.Id, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Passkey not found"
            });
        }
        
        var response = new UserPasskeyDto
        {
            Id = passkey.Id,
            DisplayName = passkey.DisplayName,
            LastSeenAt = passkey.LastSeenDate,
            CreatedAt = passkey.CreatedAt,
        };;
        return Results.Success(SuccessCodes.Ok, response);
    }
}