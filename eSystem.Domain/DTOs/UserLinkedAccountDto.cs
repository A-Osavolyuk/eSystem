using eSystem.Domain.Security.Authorization.OAuth;

namespace eSystem.Domain.DTOs;

public class UserLinkedAccountDto
{
    public Guid Id { get; set; }
    public LinkedAccountType Type { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
}