using System.Security.Claims;
using eSecurity.Idp.Security.Identity.Privacy;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Authorization.Scopes.Handlers;

public sealed class ProfileScopeHandler(
    IPersonalDataQueryService personalDataQueryService,
    IUserQueryService userQueryService) : IScopeHandler
{
    private readonly IPersonalDataQueryService _personalDataQueryService = personalDataQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;

    public bool CanHandle(string scope) => scope == ScopeTypes.Profile;

    public async ValueTask<TypedResult<List<Claim>>> HandleAsync(ScopeContext context, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userQueryService.GetByIdAsync(context.UserId, cancellationToken);
        if (user is null)
        {
            return TypedResult<List<Claim>>.Fail(new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid user"
            });
        }

        var claims = new List<Claim>();
        claims.Add(new Claim(AppClaimTypes.PreferredUsername, user.Username));
        claims.Add(new Claim(AppClaimTypes.ZoneInfo, user.ZoneInfo));
        claims.Add(new Claim(AppClaimTypes.Locale, user.Locale));

        if (user.UpdatedAt.HasValue)
        {
            var updatedAt = user.UpdatedAt.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.UpdatedAt, updatedAt, ClaimValueTypes.Integer64));
        }

        var personalData = await _personalDataQueryService.GetByUserAsync(context.UserId, cancellationToken);
        if (personalData is null)
        {
            return TypedResult<List<Claim>>.Fail(new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid personal data"
            });
        }
        
        var birthDate = personalData.BirthDate.ToString("YYYY-MM-DD");

        claims.Add(new Claim(AppClaimTypes.Name, personalData.Fullname));
        claims.Add(new Claim(AppClaimTypes.GivenName, personalData.FirstName));
        claims.Add(new Claim(AppClaimTypes.FamilyName, personalData.LastName));
        claims.Add(new Claim(AppClaimTypes.BirthDate, birthDate));
        claims.Add(new Claim(AppClaimTypes.Gender, personalData.Gender.ToString().ToLower()));

        if (!string.IsNullOrEmpty(personalData.MiddleName))
            claims.Add(new Claim(AppClaimTypes.MiddleName, personalData.MiddleName));

        return TypedResult<List<Claim>>.Success(claims);
    }
}