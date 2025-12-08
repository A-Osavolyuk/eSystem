using System.Security.Claims;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Constants;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetUserInfoQuery(string? AccessToken = null) : IRequest<Result>;

public class GetUserInfoQueryHandler(
    ITokenValidator tokenValidator,
    IUserManager userManager,
    IEmailManager emailManager,
    IPhoneManager phoneManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserInfoQuery, Result>
{
    private readonly ITokenValidator _tokenValidator = tokenValidator;
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var token = request.AccessToken;
        ;
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
                $"Bearer error=\"{Errors.OAuth.InvalidRequest}\", error_description=\"{description}\"");

            return Results.Unauthorized(new Error
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = description
            });
        }

        var validationResult = await _tokenValidator.ValidateAsync(token, cancellationToken);
        if (!validationResult.Succeeded)
        {
            var error = validationResult.GetError();

            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{error.Code}\", error_description=\"{error.Description}\"");

            return validationResult;
        }

        var claimsPrincipal = validationResult.Get<ClaimsPrincipal>();
        var scopeClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Scope);
        if (scopeClaim is null || !scopeClaim.Value.Contains(Scopes.OpenId))
        {
            const string description = "The access token does not contain the required 'openid' scope";
            
            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{Errors.OAuth.InvalidRequest}\", error_description=\"{description}\"");
            
            return Results.Forbidden(new Error
            {
                Code = Errors.OAuth.InsufficientScope,
                Description = description
            });
        }

        var subjectClaim = claimsPrincipal.Claims.First(x => x.Type == AppClaimTypes.Sub);
        var user = await _userManager.FindByIdAsync(Guid.Parse(subjectClaim.Value), cancellationToken);
        if (user is null)
        {
            const string description = "The access token is invalid or the user does not exist";

            _httpContext.Response.Headers.Append(HeaderTypes.WwwAuthenticate,
                $"Bearer error=\"{Errors.OAuth.InvalidToken}\", error_description=\"{description}\"");

            return Results.Unauthorized(new Error
            {
                Code = Errors.OAuth.InvalidToken,
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

        if (scopes.Contains(Scopes.Profile))
        {
            response.PreferredUsername = user.Username;
            response.UpdatedAt = user.UpdateDate?.ToUnixTimeSeconds();
            response.Zoneinfo = user.ZoneInfo;
            response.Locale = user.Locale;

            if (user.PersonalData is not null)
            {
                response.GivenName = user.PersonalData.FirstName;
                response.FamilyName = user.PersonalData.LastName;
                response.MiddleName = user.PersonalData.MiddleName;
                response.Birthdate = user.PersonalData.BirthDate;
                response.Gender = user.PersonalData.Gender.ToString().ToLowerInvariant();
                response.Name = user.PersonalData.Fullname;
            }
        }

        if (scopes.Contains(Scopes.Address) && user.PersonalData?.Address is not null)
        {
            response.Address = new AddressClaim()
            {
                Country = user.PersonalData.Address.Country,
                Locality = user.PersonalData.Address.Locality,
                PostalCode = user.PersonalData.Address.PostalCode,
                StreetAddress = user.PersonalData.Address.StreetAddress,
                Region = user.PersonalData.Address.Region,
            };
        }

        return Results.Ok(response);
    }
}