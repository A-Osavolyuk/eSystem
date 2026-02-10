using System.Security.Claims;
using System.Text.Json;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.Privacy;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class IdTokenClaimsContext : TokenClaimsContext
{
    public required string Aud { get; set; }
    public required string Sid { get; set; } = string.Empty;
    public string[] AuthenticationMethods { get; set; } = [];
    public string? Nonce { get; set; }
    public DateTimeOffset? AuthTime { get; set; }
}

public sealed class IdTokenClaimsFactory(
    IOptions<TokenConfigurations> options,
    IEmailManager emailManager,
    IPhoneManager phoneManager,
    IPersonalDataManager personalDataManager) : ITokenClaimsFactory<IdTokenClaimsContext, UserEntity>
{
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IPersonalDataManager _personalDataManager = personalDataManager;
    private readonly TokenConfigurations _configurations = options.Value;

    public async ValueTask<List<Claim>> GetClaimsAsync(UserEntity user,
        IdTokenClaimsContext context, CancellationToken cancellationToken)
    {
        var exp = DateTimeOffset.UtcNow.Add(_configurations.DefaultIdTokenLifetime).ToUnixTimeSeconds().ToString();
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var authTime = context.AuthTime.HasValue
            ? context.AuthTime.Value.ToUnixTimeSeconds().ToString()
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _configurations.Issuer),
            new(AppClaimTypes.Aud, context.Aud),
            new(AppClaimTypes.Sub, user.Id.ToString()),
            new(AppClaimTypes.Sid, context.Sid),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.AuthTime, authTime, ClaimValueTypes.Integer64),
        };

        if (context.AuthenticationMethods.Length > 0)
        {
            var amrValue = JsonSerializer.Serialize(context.AuthenticationMethods);
            claims.Add(new (AppClaimTypes.Amr, amrValue));
        }

        if (!string.IsNullOrEmpty(context.Nonce))
        {
            claims.Add(new(AppClaimTypes.Nonce, context.Nonce));
        }

        if (context.Scopes.Contains(ScopeTypes.Email))
        {
            var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
            if (email is not null)
            {
                claims.Add(new Claim(AppClaimTypes.Email, email.Email));
                claims.Add(new Claim(AppClaimTypes.EmailVerified, email.IsVerified.ToString()));
            }
        }

        if (context.Scopes.Contains(ScopeTypes.Phone))
        {
            var phone = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
            if (phone is not null)
            {
                claims.Add(new Claim(AppClaimTypes.PhoneNumber, phone.PhoneNumber));
                claims.Add(new Claim(AppClaimTypes.PhoneNumberVerified, phone.IsVerified.ToString()));
            }
        }

        var personalData = await _personalDataManager.GetAsync(user, cancellationToken);
        if (context.Scopes.Contains(ScopeTypes.Profile))
        {
            claims.Add(new Claim(AppClaimTypes.PreferredUsername, user.Username));
            claims.Add(new Claim(AppClaimTypes.ZoneInfo, user.ZoneInfo));
            claims.Add(new Claim(AppClaimTypes.Locale, user.Locale));

            if (user.UpdateDate.HasValue)
            {
                var updatedAt = user.UpdateDate.Value.ToUnixTimeSeconds().ToString();
                claims.Add(new Claim(AppClaimTypes.UpdatedAt, updatedAt, ClaimValueTypes.Integer64));
            }

            if (personalData is not null)
            {
                var birthDate = personalData.BirthDate.ToString("YYYY-MM-DD");

                claims.Add(new Claim(AppClaimTypes.Name, personalData.Fullname));
                claims.Add(new Claim(AppClaimTypes.GivenName, personalData.FirstName));
                claims.Add(new Claim(AppClaimTypes.FamilyName, personalData.LastName));
                claims.Add(new Claim(AppClaimTypes.BirthDate, birthDate));
                claims.Add(new Claim(AppClaimTypes.Gender, personalData.Gender.ToString().ToLower()));

                if (!string.IsNullOrEmpty(personalData.MiddleName))
                    claims.Add(new Claim(AppClaimTypes.MiddleName, personalData.MiddleName));
            }
        }

        if (context.Scopes.Contains(ScopeTypes.Address) && personalData?.Address is not null)
        {
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
        }

        return claims;
    }
}