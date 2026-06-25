using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public class SoftwareKeyManager(AuthDbContext context) : ISoftwareKeyManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<SoftwareKeyEntity>> GetAllAsync(UserEntity user,
        CancellationToken cancellationToken)
    {
        return await _context.Passkeys
            .Where(x => _context.UserDevices
                .Any(device => device.UserId == user.Id && device.Id == x.DeviceId))
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> FindByDeviceAsync(UserDeviceEntity device, CancellationToken cancellationToken)
    {
        return await _context.Passkeys
            .Where(x => x.DeviceId == device.Id)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> FindByCredentialIdAsync(string credentialId,
        CancellationToken cancellationToken)
    {
        return await _context.Passkeys
            .Where(x => x.CredentialId == credentialId)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Passkeys
            .Where(x => x.Id == id)
            .Include(x => x.Device)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> VerifyAsync(SoftwareKeyEntity softwareKey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken)
    {
        var authenticatorAssertionResponse = credential.Response;
        var clientData = ClientData.Parse(authenticatorAssertionResponse.ClientDataJson);

        if (clientData is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid client data"
            });
        }

        if (clientData.Type != ClientDataTypes.Get)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid type"
            });
        }

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        if (storedChallenge != base64Challenge)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Challenge mismatch"
            });
        }

        var valid = CredentialUtils.VerifySignature(authenticatorAssertionResponse, softwareKey.PublicKey);
        if (!valid)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid client data"
            });
        }

        var signCount = CredentialUtils.ParseSignCount(authenticatorAssertionResponse.AuthenticatorData);

        softwareKey.SignCount = signCount;
        softwareKey.LastSeenDate = DateTimeOffset.UtcNow;

        _context.Passkeys.Update(softwareKey);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return await _context.Passkeys
            .Where(x => _context.UserDevices
                .Any(device => device.UserId == user.Id && device.Id == x.DeviceId))
            .AnyAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Passkeys.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UpdateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Passkeys.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> DeleteAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Passkeys.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}