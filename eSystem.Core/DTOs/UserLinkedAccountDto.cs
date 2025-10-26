using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Core.DTOs;

public class UserLinkedAccountDto
{
    public Guid Id { get; set; }
    public LinkedAccountType Type { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
}