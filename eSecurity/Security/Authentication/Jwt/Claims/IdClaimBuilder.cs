using eSecurity.Data.Entities;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Security.Authentication.Jwt.Claims;

public sealed class IdClaimBuilder : JwtClaimBuilderBase<IdClaimBuilder>
{
    private IdClaimBuilder() { }
    public static IdClaimBuilder Create() => new();
    public IdClaimBuilder WithAuthenticationTime(DateTimeOffset date) => Add(AppClaimTypes.AuthenticationTime, date);
    public IdClaimBuilder WithAccessTokenHash(string hash) => Add(AppClaimTypes.AccessTokenHash, hash);
    public IdClaimBuilder WithAuthorizationCodeHash(string hash) => Add(AppClaimTypes.AuthorizationCodeHash, hash);

    public IdClaimBuilder WithOpenId(UserEntity user, ClientEntity client)
    {
        if (client.HasScope(Scopes.Email)) WithEmail(user);
        if (client.HasScope(Scopes.Phone)) WithPhone(user);
        if (client.HasScope(Scopes.Profile)) WithProfile(user);
        if (client.HasScope(Scopes.Address)) WithAddress(user);

        return this;
    }

    private void WithEmail(UserEntity user)
    {
        var email = user.GetEmail(EmailType.Primary);

        if (email is not null)
        {
            Add(AppClaimTypes.Email, email.Email);
            Add(AppClaimTypes.EmailVerified, email.IsVerified);
        }
    }

    private void WithPhone(UserEntity user)
    {
        var phoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);

        if (phoneNumber is not null)
        {
            Add(AppClaimTypes.PhoneNumber, phoneNumber.PhoneNumber);
            Add(AppClaimTypes.PhoneNumberVerified, phoneNumber.IsVerified);
        }
    }

    private void WithProfile(UserEntity user)
    {
        Add(AppClaimTypes.PreferredUsername, user.Username);

        if (user.UpdateDate.HasValue) 
            Add(AppClaimTypes.UpdatedAt, user.UpdateDate.Value);
        
        if (user.PersonalData is not null)
        {
            var personalData = user.PersonalData;
            Add(AppClaimTypes.GivenName, personalData.FirstName);
            Add(AppClaimTypes.FamilyName, personalData.LastName);
            Add(AppClaimTypes.MiddleName, personalData.MiddleName);
            Add(AppClaimTypes.Gender, personalData.Gender.ToString());
            Add(AppClaimTypes.BirthDate, personalData.BirthDate);
            
            var fullName = string.Join(" ", new[] { personalData.FirstName, personalData.MiddleName, personalData.LastName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            Add(AppClaimTypes.Name, fullName);
            
            //TODO: Implement user's local, zoneinfo and picture claims
        }
    }

    private void WithAddress(UserEntity user)
    {
        //TODO: Implement user address claim
    }
}