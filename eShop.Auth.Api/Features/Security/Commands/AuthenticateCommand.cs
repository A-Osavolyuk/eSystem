using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AuthenticateCommand() : IRequest<Result>;

public class AuthenticateCommandHandler(
    TokenHandler tokenHandler,
    ITokenManager tokenManager,
    IUserManager userManager) : IRequestHandler<AuthenticateCommand, Result>
{
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        var token = tokenHandler.Get();
        if (string.IsNullOrEmpty(token)) return Results.Unauthorized("Invalid token");

        var validationResult = tokenHandler.Verify(token);
        if (!validationResult.Succeeded) return validationResult;

        var claims = tokenHandler.Read(token);
        if (claims.Count == 0) return Results.Unauthorized("Invalid token");

        var subjectClaim = claims.FirstOrDefault(x => x.Type == AppClaimTypes.Subject);
        if (subjectClaim is null) return Results.Unauthorized("Invalid token");

        var userId = Guid.Parse(subjectClaim.Value);
        var user = await userManager.FindByIdAsync(userId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {userId}.");

        var accessToken = await tokenManager.GenerateAsync(user, cancellationToken);
        
        var response = new AuthenticateResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
        };

        return Result.Success(response);
    }
}