using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserVerificationDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserVerificationDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserVerificationDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.BadRequest("Invalid device.");

        var response = new UserVerificationData()
        {
            PreferredMethod = user.VerificationMethods.Single(x => x.Preferred).Method,
            EmailEnabled = user.HasVerification(VerificationMethod.Email) && user.HasEmail(EmailType.Primary),
            PasskeyEnabled = user.HasVerification(VerificationMethod.Passkey) && device.Passkey is not null,
            AuthenticatorEnabled = user.HasVerification(VerificationMethod.AuthenticatorApp),
            Methods = user.VerificationMethods.Select(method => new UserVerificationMethod()
            {
                Method = method.Method,
                Preferred = method.Preferred
            }).ToList()
        };

        if (device.Passkey is null && user.HasVerification(VerificationMethod.Passkey))
        {
            var passkeyMethod = response.Methods.Single(x => x.Method == VerificationMethod.Passkey);
            response.Methods.Remove(passkeyMethod);
        }

        return Result.Success(response);
    }
}