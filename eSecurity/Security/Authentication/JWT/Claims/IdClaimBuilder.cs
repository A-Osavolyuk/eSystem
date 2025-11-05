using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Security.Authentication.JWT.Claims;

public sealed class IdClaimBuilder : JwtClaimBuilderBase<IdClaimBuilder>
{
    private IdClaimBuilder() {}
    public static IdClaimBuilder Create() => new();
    
    public IdClaimBuilder WithName(string name) => Add(AppClaimTypes.Name, name);
    public IdClaimBuilder WithNickname(string nickname) => Add(AppClaimTypes.Nickname, nickname);
    public IdClaimBuilder WithGivenName(string givenName) => Add(AppClaimTypes.GivenName, givenName);
    public IdClaimBuilder WithFamilyName(string familyName) => Add(AppClaimTypes.FamilyName, familyName);
    public IdClaimBuilder WithMiddleName(string middleName) => Add(AppClaimTypes.MiddleName, middleName);
    public IdClaimBuilder WithPreferredUsername(string username) => Add(AppClaimTypes.PreferredUsername, username);
    public IdClaimBuilder WithEmail(string email) => Add(AppClaimTypes.Email, email);
    public IdClaimBuilder WithEmailVerified(bool verified) => Add(AppClaimTypes.EmailVerified, verified);
    public IdClaimBuilder WithPhoneNumber(string number) => Add(AppClaimTypes.PhoneNumber, number);
    public IdClaimBuilder WithPhoneNumberVerified(bool verified) => Add(AppClaimTypes.PhoneNumberVerified, verified);
    public IdClaimBuilder WithLocale(string locale) => Add(AppClaimTypes.Locale, locale);
    public IdClaimBuilder WithZoneInfo(string zone) => Add(AppClaimTypes.ZoneInfo, zone);
    public IdClaimBuilder WithAddress(string address) => Add(AppClaimTypes.Address, address);
    public IdClaimBuilder WithGender(string gender) => Add(AppClaimTypes.Gender, gender);
    public IdClaimBuilder WithPicture(string picture) => Add(AppClaimTypes.Picture, picture);
    public IdClaimBuilder WithProfile(string profile) => Add(AppClaimTypes.Profile, profile);
    public IdClaimBuilder WithBirthDate(DateTimeOffset date) => Add(AppClaimTypes.BirthDate, date);
    public IdClaimBuilder WithUpdatedTime(DateTimeOffset date) => Add(AppClaimTypes.UpdatedAt, date);
    public IdClaimBuilder WithAuthenticationTime(DateTimeOffset date) => Add(AppClaimTypes.AuthenticationTime, date);
    public IdClaimBuilder WithAccessTokenHash(string hash) => Add(AppClaimTypes.AccessTokenHash, hash);
    public IdClaimBuilder WithAuthorizationCodeHash(string hash) => Add(AppClaimTypes.AuthorizationCodeHash, hash);
}