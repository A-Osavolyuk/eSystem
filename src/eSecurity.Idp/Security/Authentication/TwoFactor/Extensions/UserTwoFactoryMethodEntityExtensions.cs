using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.TwoFactor.Extensions;

public static class UserTwoFactoryMethodEntityExtensions
{
    public static TwoFactorMethodInfo ToInfo(this UserTwoFactorMethodEntity entity)
    {
        return new TwoFactorMethodInfo(entity.Method.Type, entity.Preferred,
            TwoFactorHelper.GetMethodPriority(entity.Method.Type));
    }
}