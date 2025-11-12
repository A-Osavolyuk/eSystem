using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Data;

public static class UserEntityExtensions
{
    extension(UserEntity user)
    {
        public UserDeviceEntity? GetDevice(string userAgent, string ipAddress)
            => user.Devices.FirstOrDefault(x => x.UserAgent == userAgent && x.IpAddress == ipAddress);

        public UserLinkedAccountEntity? GetLinkedAccount(LinkedAccountType type)
            => user.LinkedAccounts.FirstOrDefault(x => x.Type == type);

        public UserEmailEntity? GetEmail(EmailType type)
            => user.Emails.FirstOrDefault(x => x.Type == type);

        public UserEmailEntity? GetEmail(string email)
            => user.Emails.FirstOrDefault(x => x.Email == email);

        public UserPhoneNumberEntity? GetPhoneNumber(PhoneNumberType type)
            => user.PhoneNumbers.FirstOrDefault(x => x.Type == type);

        public UserTwoFactorMethodEntity? GetTwoFactorMethod(TwoFactorMethod method)
            => user.TwoFactorMethods.FirstOrDefault(x => x.Method == method);

        public UserVerificationMethodEntity? GetVerificationMethod(VerificationMethod method)
            => user.VerificationMethods.FirstOrDefault(x => x.Method == method);

        public int CountPasskeys() 
            => user.Devices.Select(x => x.Passkey).Count();
    }
}