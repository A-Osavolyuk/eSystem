using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Data;

public static class UserEntityExtensions
{
    extension(UserEntity user)
    {
        public UserLinkedAccountEntity? GetLinkedAccount(LinkedAccountType type)
            => user.LinkedAccounts.FirstOrDefault(x => x.Type == type);

        public UserTwoFactorMethodEntity? GetTwoFactorMethod(TwoFactorMethod method)
            => user.TwoFactorMethods.FirstOrDefault(x => x.Method == method);

        public UserVerificationMethodEntity? GetVerificationMethod(VerificationMethod method)
            => user.VerificationMethods.FirstOrDefault(x => x.Method == method);
    }
}