using System.Security.Claims;
using System.Text.Json;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Phone;
using eSecurity.Idp.Security.Identity.Privacy;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Id;

public sealed class IdTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider,
    IEmailManager emailManager,
    IPhoneManager phoneManager,
    IPersonalDataManager personalDataManager) : ITokenFactory<IdTokenFactoryContext>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IPersonalDataManager _personalDataManager = personalDataManager;

    public async ValueTask<TypedResult<string>> CreateAsync(
        IdTokenFactoryContext context,
        TokenFactoryOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        List<string> scopes;
        if (options is null || options.AllowedScopes.Count == 0)
        {
            scopes = context.Client.AllowedScopes
                .Select(x => x.Scope.Value)
                .ToList();
        }
        else
        {
            scopes = context.Client.AllowedScopes
                .Where(x => options.AllowedScopes.Contains(x.Scope.Value))
                .Select(x => x.Scope.Value)
                .ToList();
        }
        
        var subjectResult = await _subjectProvider.GetSubjectAsync(context.User, context.Client, cancellationToken);
        if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
        {
            return TypedResult<string>.Fail(new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var tokenLifetime = context.Client.IdTokenLifetime ?? _tokenConfigurations.DefaultIdTokenLifetime;
        var authenticationMethods = context.Session.AuthenticationMethods
            .Select(x => x.MethodReference.GetString())
            .ToArray();
        
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var exp = DateTimeOffset.UtcNow.Add(tokenLifetime).ToString();
        var authTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Aud, context.Client.Id.ToString()),
            new(AppClaimTypes.Sub, subject),
            new(AppClaimTypes.Sid, context.Session.Id.ToString()),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.AuthTime, authTime, ClaimValueTypes.Integer64),
        };

        if (authenticationMethods.Length > 0)
        {
            var amrValue = JsonSerializer.Serialize(authenticationMethods);
            claims.Add(new Claim(AppClaimTypes.Amr, amrValue));
        }

        if (!string.IsNullOrEmpty(options?.Nonce))
            claims.Add(new Claim(AppClaimTypes.Nonce, options.Nonce));

        if (scopes.Contains(ScopeTypes.Email))
        {
            var email = await _emailManager.FindByTypeAsync(context.User, EmailType.Primary, cancellationToken);
            if (email is not null)
            {
                claims.Add(new Claim(AppClaimTypes.Email, email.Email));
                claims.Add(new Claim(AppClaimTypes.EmailVerified, email.IsVerified.ToString()));
            }
        }

        if (scopes.Contains(ScopeTypes.Phone))
        {
            var phone = await _phoneManager.FindByTypeAsync(context.User, PhoneNumberType.Primary, cancellationToken);
            if (phone is not null)
            {
                claims.Add(new Claim(AppClaimTypes.PhoneNumber, phone.PhoneNumber));
                claims.Add(new Claim(AppClaimTypes.PhoneNumberVerified, phone.IsVerified.ToString()));
            }
        }

        var personalData = await _personalDataManager.GetAsync(context.User, cancellationToken);
        if (scopes.Contains(ScopeTypes.Profile))
        {
            claims.Add(new Claim(AppClaimTypes.PreferredUsername, context.User.Username));
            claims.Add(new Claim(AppClaimTypes.ZoneInfo, context.User.ZoneInfo));
            claims.Add(new Claim(AppClaimTypes.Locale, context.User.Locale));

            if (context.User.UpdatedAt.HasValue)
            {
                var updatedAt = context.User.UpdatedAt.Value.ToUnixTimeSeconds().ToString();
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

        if (scopes.Contains(ScopeTypes.Address) && personalData?.Address is not null)
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

        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.IdToken };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}