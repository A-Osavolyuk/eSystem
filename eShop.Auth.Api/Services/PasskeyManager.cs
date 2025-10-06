using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Common.Security.Credentials;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IPasskeyManager), ServiceLifetime.Scoped)]
public class PasskeyManager(AuthDbContext context) : IPasskeyManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<UserPasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken)
    {
        var entity = await context.UserPasskeys
            .FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);
        
        return entity;
    }
    
    public async ValueTask<UserPasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.UserPasskeys
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity;
    }

    public async ValueTask<Result> VerifyAsync(UserPasskeyEntity passkey, PublicKeyCredential credential, 
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
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        context.UserPasskeys.Update(passkey);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        await context.UserPasskeys.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserPasskeys.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserPasskeys.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}