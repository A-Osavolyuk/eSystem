using System.Security.Claims;
using System.Text.Json;
using eSecurity.Idp.Security.Identity.Privacy;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Authorization.Scopes.Handlers;

public sealed class AddressScopeHandler(IPersonalDataQueryService personalDataQueryService) : IScopeHandler
{
    private readonly IPersonalDataQueryService _personalDataQueryService = personalDataQueryService;
    
    public bool CanHandle(string scope) => scope == ScopeTypes.Address;
    
    public async ValueTask<TypedResult<List<Claim>>> HandleAsync(ScopeContext context, 
        CancellationToken cancellationToken = default)
    {
        var personalData = await _personalDataQueryService.GetByUserAsync(context.UserId, cancellationToken);
        if (personalData?.Address is null)
        {
            return TypedResult<List<Claim>>.Fail(new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid personal data"
            });
        }

        var claims = new List<Claim>();
        var claim = new AddressClaim
        {
            Country = personalData.Address.Country,
            Locality = personalData.Address.Locality,
            PostalCode = personalData.Address.PostalCode,
            Region = personalData.Address.Region,
            StreetAddress = personalData.Address.StreetAddress,
        };

        var claimJson = JsonSerializer.Serialize(claim);
        claims.Add(new Claim(AppClaimTypes.Address, claimJson));

        return TypedResult<List<Claim>>.Success(claims);
    }
}