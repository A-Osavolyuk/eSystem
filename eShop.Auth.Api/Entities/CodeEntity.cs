namespace eShop.Auth.Api.Entities;

public class CodeEntity : Entity, IExpirable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public Guid UserId { get; init; }
    public string CodeHash { get; init; } = string.Empty;
    public CodeType Type { get; init; }
    public SenderType Sender { get; init; }
    public CodeResource Resource { get; init; }
    public DateTimeOffset ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);

    public UserEntity? User { get; init; }
}