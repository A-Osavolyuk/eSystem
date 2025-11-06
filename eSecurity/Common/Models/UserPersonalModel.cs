using eSecurity.Security.Identity.Privacy;

namespace eSecurity.Common.Models;

public class UserPersonalModel
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}