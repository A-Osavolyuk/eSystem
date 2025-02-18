using System.ComponentModel.DataAnnotations;

namespace eShop.Auth.Api.Entities;

public class CodeEntity : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string SentTo { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public VerificationCodeType VerificationCodeType { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddMinutes(10);
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}