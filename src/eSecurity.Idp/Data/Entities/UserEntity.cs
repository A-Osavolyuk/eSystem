using eSecurity.Idp.Common.Validation;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserEntity() : Entity
{
    public UserEntity(string username) : this()
    {
        Id = Guid.CreateVersion7();
        Username = username;
        NormalizedUsername = Normalizer.Normalize(username);
        AccountConfirmed = true;
    }
    
    public Guid Id { get; init; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public DateTimeOffset? UsernameChangeDate { get; set; }
    public bool AccountConfirmed { get; init; }
    public int FailedLoginAttempts { get; set; }

    public int ResendAttempts { get; set; }
    public DateTimeOffset? ResendAvailableAt { get; set; }

    public string ZoneInfo { get; init; } = string.Empty;
    public string Locale { get; init; } = string.Empty;

    public PublicSubjectEntity PublicSubject { get; set; } = null!;
    public ICollection<UserEmailEntity> Emails { get; set; } = null!;
}