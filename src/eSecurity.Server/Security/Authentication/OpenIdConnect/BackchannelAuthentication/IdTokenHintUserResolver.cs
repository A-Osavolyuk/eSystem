using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token.Validation;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
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

    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.IdTokenHint))
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "id_token_hind is invalid"
            });
        }
        
        var validator = _validationProvider.CreateValidator(JwtTokenTypes.IdToken);
        var validationResult = await validator.ValidateAsync(request.IdTokenHint, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            {
                return TypedResult<UserEntity>.Fail(new Error()
                {
                    Code = ErrorTypes.OAuth.InvalidRequest,
                    Description = "id_token_hind is invalid"
                });
            }
        }

        var subClaim = validationResult.ClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sub);
        if (subClaim is null)
        {
            {
                return TypedResult<UserEntity>.Fail(new Error()
                {
                    Code = ErrorTypes.OAuth.InvalidRequest,
                    Description = "id_token_hind is invalid"
                });
            }
        }
        
        var user = await _userManager.FindByIdAsync(Guid.Parse(subClaim.Value), cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        return TypedResult<UserEntity>.Success(user);
    }
}