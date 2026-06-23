using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Client;

public sealed class ClientCommandService(AuthDbContext context) : IClientCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> RelateAsync(Guid clientId, Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.ClientSessions.AnyAsync(
            x => x.SessionId == sessionId && x.ClientId == clientId, cancellationToken);

        if (exists)
            return Results.Success(SuccessCodes.Ok);

        var entity = new ClientSessionEntity
        {
            ClientId = clientId,
            SessionId = sessionId
        };

        await _context.ClientSessions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}