using eShop.Domain.Interfaces;

namespace eShop.Auth.Api.Entities;

public class VerificationCodeEntity : IEntity<Guid>, IExpireable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Destination { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public Verification CodeType { get; init; }
    public DateTime ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}