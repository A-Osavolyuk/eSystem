namespace eAccount.Domain.Interfaces;

public interface INotificationService
{
    public ValueTask<int> GetNotificationsCountAsync();
    public ValueTask SetNotificationsCountAsync(int notificationsCount);
    public ValueTask IncrementNotificationsCountAsync();
    public ValueTask DecrementNotificationsCountAsync();
}