namespace eShop.Domain.DTOs;

public class SellerDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
}