using System.Security.Cryptography;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public sealed class SoftwareKeyCommandService(AuthDbContext context) : ISoftwareKeyCommandService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.SoftwareKeys.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> DeleteAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.SoftwareKeys.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> VerifyAsync(SoftwareKeyEntity entity, PublicKeyCredential credential,
        string savedChallenge, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentException.ThrowIfNullOrWhiteSpace(savedChallenge);
        
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

        if (clientData.Type != ClientDataType.Get)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid type"
            });
        }
        
        var savedChallengeBytes = Convert.FromBase64String(savedChallenge);
        if (!CryptographicOperations.FixedTimeEquals(savedChallengeBytes, clientData.Challenge))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidChallenge,
                Description = "Challenge mismatch"
            });
        }
        
        if (!CredentialUtils.VerifySignature(authenticatorAssertionResponse, entity.PublicKey))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid client data"
            });
        }

        var signCount = CredentialUtils.ParseSignCount(authenticatorAssertionResponse.AuthenticatorData);

        entity.SignCount = signCount;
        entity.LastSeenDate = DateTimeOffset.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ChangeDisplayNameAsync(SoftwareKeyEntity entity, string displayName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        entity.DisplayName = displayName;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}