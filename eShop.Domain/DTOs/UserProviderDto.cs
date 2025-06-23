namespace eShop.Domain.DTOs;

public class UserProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Subscribed { get; set; }
}