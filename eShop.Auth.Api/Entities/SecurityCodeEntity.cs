namespace eShop.Auth.Api.Entities;

public class SecurityCodeEntity : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; }
    public string Destination { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public DateTime ExpireDate { get; init; } = DateTime.UtcNow.AddMinutes(10);
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}