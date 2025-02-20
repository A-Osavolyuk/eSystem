using System.ComponentModel.DataAnnotations;

namespace eShop.Auth.Api.Entities;

public class VerificationCodeEntity : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Destination { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public VerificationCodeType CodeType { get; init; }
    public DateTime ExpireDate { get; init; } = DateTime.UtcNow.AddMinutes(10);
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}