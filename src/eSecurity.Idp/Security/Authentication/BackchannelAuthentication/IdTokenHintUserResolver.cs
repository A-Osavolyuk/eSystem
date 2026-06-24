using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Features.Connect;
using eSecurity.Idp.Security.Authorization.Token.Validation;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public sealed class IdTokenHintUserResolver(
    IJwtTokenValidationProvider validationProvider,
    IUserQueryService userQueryService) : IUserResolver
{
    private readonly IJwtTokenValidationProvider _validationProvider = validationProvider;
    private readonly IUserQueryService _userQueryService = userQueryService;

    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.IdTokenHint))
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "id_token_hind is invalid"
            });
        }
        
        var validator = _validationProvider.CreateValidator(JwtTokenType.IdToken);
        var validationResult = await validator.ValidateAsync(command.IdTokenHint, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            {
                return TypedResult<UserEntity>.Fail(new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "id_token_hind is invalid"
                });
            }
        }

        var subClaim = validationResult.ClaimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Sub);
        if (subClaim is null)
        {
            {
                return TypedResult<UserEntity>.Fail(new Error
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = "id_token_hind is invalid"
                });
            }
        }
        
        var user = await _userQueryService.GetByIdAsync(Guid.Parse(subClaim.Value), cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        return TypedResult<UserEntity>.Success(user);
    }
}