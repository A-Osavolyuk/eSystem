using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserVerificationDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserVerificationDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserVerificationDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var userAgent = httpContext.GetUserAgent();
        var ip = httpContext.GetIpV4();
        
        var device = await deviceManager.FindAsync(user, userAgent, ip, cancellationToken);
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

        return Result.Success(response);
    }
}