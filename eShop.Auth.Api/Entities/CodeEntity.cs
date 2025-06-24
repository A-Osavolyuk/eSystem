namespace eShop.Auth.Api.Entities;

public class CodeEntity : IEntity, IExpireable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public Guid UserId { get; init; }
    public string Code { get; init; } = string.Empty;
    public CodeType Type { get; init; }
    public SenderType Sender { get; set; }
    
    public DateTimeOffset ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    

    public UserEntity? User { get; init; }
}