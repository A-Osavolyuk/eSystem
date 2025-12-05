using System.Text.Json;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Builders;

public sealed class IdClaimBuilder : JwtClaimBuilderBase<IdClaimBuilder>
{
    private IdClaimBuilder() { }
    public static IdClaimBuilder Create() => new();
    public IdClaimBuilder WithAuthenticationTime(DateTimeOffset date) => Add(AppClaimTypes.AuthenticationTime, date);
    public IdClaimBuilder WithAccessTokenHash(string hash) => Add(AppClaimTypes.AccessTokenHash, hash);
    public IdClaimBuilder WithAuthorizationCodeHash(string hash) => Add(AppClaimTypes.AuthorizationCodeHash, hash);

    public IdClaimBuilder WithOpenId(UserEntity user, List<string> scopes)
    {
        if (scopes.Contains(Scopes.Email)) WithEmail(user);
        if (scopes.Contains(Scopes.Phone)) WithPhone(user);
        if (scopes.Contains(Scopes.Profile)) WithProfile(user);
        if (scopes.Contains(Scopes.Address)) WithAddress(user);

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
        Add(AppClaimTypes.ZoneInfo, user.ZoneInfo);
        Add(AppClaimTypes.Locale, user.Locale);

        if (user.UpdateDate.HasValue) 
            Add(AppClaimTypes.UpdatedAt, user.UpdateDate.Value);
        
        if (user.PersonalData is not null)
        {
            Add(AppClaimTypes.GivenName, user.PersonalData.FirstName);
            Add(AppClaimTypes.FamilyName, user.PersonalData.LastName);
            Add(AppClaimTypes.MiddleName, user.PersonalData.MiddleName);
            Add(AppClaimTypes.Gender, user.PersonalData.Gender.ToString().ToLowerInvariant());
            Add(AppClaimTypes.BirthDate, user.PersonalData.BirthDate);
            Add(AppClaimTypes.Name, user.PersonalData.Fullname);
        }
    }

    private void WithAddress(UserEntity user)
    {
        if (user.PersonalData?.Address is not null)
        {
            var claim = new AddressClaim()
            {
                Country = user.PersonalData.Address.Country,
                Locality = user.PersonalData.Address.Locality,
                PostalCode = user.PersonalData.Address.PostalCode,
                StreetAddress = user.PersonalData.Address.StreetAddress,
                Region = user.PersonalData.Address.Region,
            };
            
            var claimJson = JsonSerializer.Serialize(claim);
            Add(AppClaimTypes.Address, claimJson);
        }
    }
}