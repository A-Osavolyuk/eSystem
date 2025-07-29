namespace eShop.Domain.DTOs;

public class UserProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Subscribed { get; set; }
    
    public DateTimeOffset? SubscribedDate { get; set; }
    public DateTimeOffset? UnsubscribedDate { get; set; }
}