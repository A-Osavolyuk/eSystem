using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailPolicyService
{
    Result CanChangeExistingEmail(Guid userId, UserEmailEntity currentEmail, UserEmailEntity newEmail);
    Result CanChangeWithNewEmail(Guid userId, UserEmailEntity currentEmail);
}