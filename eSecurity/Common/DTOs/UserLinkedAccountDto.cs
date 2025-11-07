using eSecurity.Security.Authorization.OAuth;

namespace eSecurity.Common.DTOs;

public class UserLinkedAccountDto
{
    public Guid Id { get; set; }
    public LinkedAccountType Type { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
}