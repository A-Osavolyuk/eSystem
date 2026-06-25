using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailPolicy
{
    Result CanChangeExistingEmail(Guid userId, EmailInfo currentEmail, EmailInfo newEmail);
    Result CanChangeWithNewEmail(Guid userId, EmailInfo currentEmail);
    Result CanReset(Guid userId, EmailInfo currentEmail);
    Result CanAdd(List<EmailInfo> userEmails, EmailType type);
}