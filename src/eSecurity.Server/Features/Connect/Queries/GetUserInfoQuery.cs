using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.Privacy;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.User;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetUserInfoQuery(string? AccessToken = null) : IRequest<Result>;

public class GetUserInfoQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IPhoneManager phoneManager,
    IPersonalDataManager personalDataManager,
    IHttpContextAccessor httpContextAccessor,
    ITokenValidationProvider validationProvider) : IRequestHandler<GetUserInfoQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IPersonalDataManager _personalDataManager = personalDataManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly ITokenValidator _validator = validationProvider.CreateValidator(TokenTypes.Jwt);

    public async Task<Result> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var token = request.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            var header = _httpContext.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(header) && header.StartsWith($"{JwtBearerDefaults.AuthenticationScheme} "))
                token = header[$"{JwtBearerDefaults.AuthenticationScheme} ".Length..].Trim();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            const string description = "Missing access_token";

            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{ErrorTypes.OAuth.InvalidRequest}\", error_description=\"{description}\"");

            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = description
            });
        }

        var validationResult = await _validator.ValidateAsync(token, cancellationToken);
        if (!validationResult.IsValid || validationResult.ClaimsPrincipal is null)
        {
            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{ErrorTypes.OAuth.InvalidToken}\", error_description=\"Invalid token\"");

            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = "Invalid token"
            });
        }

        var claimsPrincipal = validationResult.ClaimsPrincipal;
        var scopeClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Scope);
        if (scopeClaim is null || !scopeClaim.Value.Contains(Scopes.OpenId))
        {
            const string description = "The access token does not contain the required 'openid' scope";
            
            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{ErrorTypes.OAuth.InvalidRequest}\", error_description=\"{description}\"");
            
            return Results.Forbidden(new Error
            {
                Code = ErrorTypes.OAuth.InsufficientScope,
                Description = description
            });
        }

        var subjectClaim = claimsPrincipal.Claims.First(x => x.Type == AppClaimTypes.Sub);
        var user = await _userManager.FindByIdAsync(Guid.Parse(subjectClaim.Value), cancellationToken);
        if (user is null)
        {
            const string description = "The access token is invalid or the user does not exist";

            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{ErrorTypes.OAuth.InvalidToken}\", error_description=\"{description}\"");

            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = description
            });
        }

        var response = new UserInfoResponse() { Subject = subjectClaim.Value };
        var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (scopes.Contains(Scopes.Email))
        {
            var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
            if (email is not null)
            {
                response.Email = email.Email;
                response.EmailVerified = email.IsVerified;
            }
        }

        if (scopes.Contains(Scopes.Phone))
        {
            var phoneNumber = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
            if (phoneNumber is not null)
            {
                response.PhoneNumber = phoneNumber.PhoneNumber;
                response.PhoneNumberVerified = phoneNumber.IsVerified;
            }
        }

        var personalData = await _personalDataManager.GetAsync(user, cancellationToken);
        if (scopes.Contains(Scopes.Profile))
        {
            response.PreferredUsername = user.Username;
            response.UpdatedAt = user.UpdateDate?.ToUnixTimeSeconds();
            response.Zoneinfo = user.ZoneInfo;
            response.Locale = user.Locale;

            if (personalData is not null)
            {
                response.GivenName = personalData.FirstName;
                response.FamilyName = personalData.LastName;
                response.MiddleName = personalData.MiddleName;
                response.Birthdate = personalData.BirthDate;
                response.Gender = personalData.Gender.ToString().ToLowerInvariant();
                response.Name = personalData.Fullname;
            }
        }

        if (scopes.Contains(Scopes.Address) && personalData?.Address is not null)
        {
            response.Address = new AddressClaim()
            {
                Country = personalData.Address.Country,
                Locality = personalData.Address.Locality,
                PostalCode = personalData.Address.PostalCode,
                StreetAddress = personalData.Address.StreetAddress,
                Region = personalData.Address.Region,
            };
        }

        return Results.Ok(response);
    }
}