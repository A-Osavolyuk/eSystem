using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;

namespace eSecurity.Server.Security.Credentials.PublicKey;

public class PasskeyManager(AuthDbContext context) : IPasskeyManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken)
    {
        var entity = await context.Passkeys
            .Where(x => x.CredentialId == credentialId)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(cancellationToken);
        
        return entity;
    }
    
    public async ValueTask<PasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Passkeys
            .Where(x => x.Id == id)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(cancellationToken);
        
        return entity;
    }

    public async ValueTask<Result> VerifyAsync(PasskeyEntity passkey, PublicKeyCredential credential, 
        string storedChallenge, CancellationToken cancellationToken)
    {
        var authenticatorAssertionResponse = credential.Response;
        var clientData = ClientData.Parse(authenticatorAssertionResponse.ClientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Get) return Results.BadRequest("Invalid type");

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        if (storedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var valid = CredentialUtils.VerifySignature(authenticatorAssertionResponse, passkey.PublicKey);
        if (!valid) return Results.BadRequest("Invalid client data");

        var signCount = CredentialUtils.ParseSignCount(authenticatorAssertionResponse.AuthenticatorData);

        passkey.SignCount = signCount;
        passkey.LastSeenDate = DateTimeOffset.UtcNow;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        context.Passkeys.Update(passkey);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        await context.Passkeys.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.Passkeys.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(PasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.Passkeys.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}