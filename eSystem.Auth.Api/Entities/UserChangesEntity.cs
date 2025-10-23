namespace eSystem.Auth.Api.Entities;

public class UserChangesEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public int Version { get; set; }
    public string? Value { get; set; }
    public ChangeField Field { get; set; }

    public UserEntity User { get; set; } = null!;
}