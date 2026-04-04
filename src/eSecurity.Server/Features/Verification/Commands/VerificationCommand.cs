using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Authorization.Verification.AuthenticationApp;
using eSecurity.Server.Security.Authorization.Verification.Passkey;
using eSecurity.Server.Security.Authorization.Verification.Totp;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Features.Verification.Commands;

public sealed record VerificationCommand(VerificationRequest Request) : IRequest<Result>;

public class VerificationCommandHandler(
    IVerificationStrategyResolver verificationStrategyResolver) : IRequestHandler<VerificationCommand, Result>
{
    private readonly IVerificationStrategyResolver _verificationStrategyResolver = verificationStrategyResolver;

    public async Task<Result> Handle(VerificationCommand request, CancellationToken cancellationToken)
    {
        VerificationContext? context = request.Request.Payload switch
        {
            TotpVerificationPayload payload => new TotpVerificationContext()
            {
                Code = payload.Code,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            PasskeyVerificationPayload payload => new PasskeyVerificationContext()
            {
                Credential = payload.Credential,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            AuthenticatorAppVerificationPayload payload => new AuthenticatorAppVerificationContext()
            {
                Code = payload.Code,
                Action = request.Request.Action,
                Purpose = request.Request.Purpose
            },
            _ => null
        };

        if (context is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid payload"
            });
        }

        var strategy = _verificationStrategyResolver.Resolve(context);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}