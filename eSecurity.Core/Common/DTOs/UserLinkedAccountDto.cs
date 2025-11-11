using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public class UserLinkedAccountDto
{
    public Guid Id { get; set; }
    public LinkedAccountType Type { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
}