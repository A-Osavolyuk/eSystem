using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.Email.Extensions;

public static class UserEmailEntityExtensions
{
    public static EmailInfo ToInfo(this UserEmailEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new EmailInfo(entity.Type, entity.IsVerified, entity.UserId);
    }
}