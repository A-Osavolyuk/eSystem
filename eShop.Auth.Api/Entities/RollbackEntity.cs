namespace eShop.Auth.Api.Entities;

public class RollbackEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Value { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    
    public RollbackField Field { get; set; }
    public RollbackAction Action { get; set; }

    public UserEntity User { get; set; } = null!;
}