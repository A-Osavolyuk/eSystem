using eShop.Infrastructure.State;

namespace eShop.Infrastructure.Services;

public class NotificationService(
    IStorage localStorage,
    NotificationsStateContainer notificationsStateContainer) : INotificationService
{
    private readonly IStorage localStorage = localStorage;
    private readonly NotificationsStateContainer notificationsStateContainer = notificationsStateContainer;
    private const string Key = "notifications-count";

    public async ValueTask<int> GetNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(Key))
        {
            return await localStorage.GetAsync<int>(Key);
        }
        else
        {
            var count = await localStorage.GetAsync<int>(Key);
            await localStorage.SetAsync(Key, count);
            return count;
        }
    }

    public async ValueTask SetNotificationsCountAsync(int notificationsCount) => await localStorage.SetAsync(Key, notificationsCount);

    public async ValueTask IncrementNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(Key))
        {
            var count = await localStorage.GetAsync<int>(Key);
            count++;
            await localStorage.SetAsync(Key, count);
            notificationsStateContainer.ChangeNotificationCount();
        }
    }

    public async ValueTask DecrementNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(Key))
        {
            var count = await localStorage.GetAsync<int>(Key);

            if (count > 0)
            {
                count--;
                await localStorage.SetAsync(Key, count);
                notificationsStateContainer.ChangeNotificationCount();
            }
        }
    }
}