using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Data;

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
        => user.TwoFactorMethods.FirstOrDefault(x => x.Method == method);

    public static UserVerificationMethodEntity? GetVerificationMethod(this UserEntity user, VerificationMethod method)
        => user.VerificationMethods.FirstOrDefault(x => x.Method == method);
    public static int CountPasskeys(this UserEntity user) 
        => user.Devices.Select(x => x.Passkey).Count();
}