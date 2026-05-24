using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Verification.AuthenticationApp;
using eSecurity.Idp.Security.Authorization.Verification.Passkey;
using eSecurity.Idp.Security.Authorization.Verification.Totp;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.Commands;

public sealed record VerificationCommand(VerificationRequest Request) : IRequest<Result>;

public class VerificationCommandHandler(
    IVerificationStrategyResolver verificationStrategyResolver) : IRequestHandler<VerificationCommand, Result>
{
    private readonly IVerificationStrategyResolver _verificationStrategyResolver = verificationStrategyResolver;

    public async Task<Result> Handle(VerificationCommand request, CancellationToken cancellationToken)
    {
        VerificationContext? context = request.Request.Payload switch
        {
            TotpVerificationPayload payload => new TotpVerificationContext
            {
                Code = payload.Code,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            PasskeyVerificationPayload payload => new PasskeyVerificationContext
            {
                Credential = payload.Credential,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            AuthenticatorAppVerificationPayload payload => new AuthenticatorAppVerificationContext
            {
                Code = payload.Code,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            _ => null
        };

        if (context is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid payload"
            });
        }

        var strategy = _verificationStrategyResolver.Resolve(context);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}