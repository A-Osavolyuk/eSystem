using eSystem.Domain.Security.Authentication.TwoFactor;
using eSystem.Domain.Security.Authorization.OAuth;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Extensions;

public static class UserEntityExtensions
{
    public static UserDeviceEntity? GetDevice(this UserEntity user, string userAgent, string ipAddress)
        => user.Devices.FirstOrDefault(x => x.UserAgent == userAgent && x.IpAddress == ipAddress);

    public static UserLinkedAccountEntity? GetLinkedAccount(this UserEntity user, LinkedAccountType type)
        => user.LinkedAccounts.FirstOrDefault(x => x.Type == type);

    public static UserEmailEntity? GetEmail(this UserEntity user, EmailType type)
        => user.Emails.FirstOrDefault(x => x.Type == type);

    public static UserEmailEntity? GetEmail(this UserEntity user, string email)
        => user.Emails.FirstOrDefault(x => x.Email == email);

    public static UserPhoneNumberEntity? GetPhoneNumber(this UserEntity user, PhoneNumberType type)
        => user.PhoneNumbers.FirstOrDefault(x => x.Type == type);

    public static UserTwoFactorMethodEntity? GetTwoFactorMethod(this UserEntity user, TwoFactorMethod method)
        => user.Methods.FirstOrDefault(x => x.Method == method);

    public static UserVerificationMethodEntity? GetVerificationMethod(this UserEntity user, VerificationMethod method)
        => user.VerificationMethods.FirstOrDefault(x => x.Method == method);
    public static int CountPasskeys(this UserEntity user) 
        => user.Devices.Select(x => x.Passkey).Count();
}