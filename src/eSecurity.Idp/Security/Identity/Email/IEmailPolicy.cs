using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailPolicy
{
    Result CanChangeExistingEmail(Guid userId, UserEmailEntity currentEmail, UserEmailEntity newEmail);
    Result CanChangeWithNewEmail(Guid userId, UserEmailEntity currentEmail);
    Result CanReset(Guid userId, UserEmailEntity currentEmail);
    Result CanAdd(List<UserEmailEntity> userEmails, EmailType type);
}