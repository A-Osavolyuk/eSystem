using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Passkeys.Queries;

public record GetPasskeyQuery(Guid Id) : IRequest<Result>;

public class GetPasskeyQueryHandler(IPasskeyManager passkeyManager) : IRequestHandler<GetPasskeyQuery, Result>
{
    private readonly IPasskeyManager _passkeyManager = passkeyManager;

    public async Task<Result> Handle(GetPasskeyQuery request, CancellationToken cancellationToken)
    {
        var passkey = await _passkeyManager.FindByIdAsync(request.Id, cancellationToken);
        if (passkey is null) return Results.NotFound("Passkey not found");
        
        var response = new UserPasskeyDto
        {
            Id = passkey.Id,
            DisplayName = passkey.DisplayName,
            LastSeenDate = passkey.LastSeenDate,
            CreateDate = passkey.CreateDate,
        };;
        return Results.Ok(response);
    }
}