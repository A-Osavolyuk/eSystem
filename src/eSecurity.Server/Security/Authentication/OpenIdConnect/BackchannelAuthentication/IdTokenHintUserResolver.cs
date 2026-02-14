using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class IdTokenHintUserResolver(
    IJwtTokenValidationProvider validationProvider,
    IUserManager userManager) : IUserResolver
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly IUserManager _userManager = userManager;

    public async Task<UserEntity?> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.IdTokenHint))
            return null;
        
        var validator = _validationProvider.CreateValidator(JwtTokenTypes.IdToken);
        var validationResult = await validator.ValidateAsync(request.IdTokenHint, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null) return null;

        var subClaim = validationResult.ClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sub);
        if (subClaim is null) return null;
        
        return await _userManager.FindByIdAsync(Guid.Parse(subClaim.Value), cancellationToken);
    }
}