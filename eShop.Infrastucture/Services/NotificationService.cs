using eShop.Infrastructure.State;

namespace eShop.Infrastructure.Services;

public class NotificationService(
    IStorage localStorage,
    NotificationsStateContainer notificationsStateContainer) : INotificationService
{
    private readonly IStorage localStorage = localStorage;
    private readonly NotificationsStateContainer notificationsStateContainer = notificationsStateContainer;
    private readonly string key = "notifications-count";

    public async ValueTask<int> GetNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(key))
        {
            return await localStorage.GetAsync<int>(key);
        }
        else
        {
            var count = await localStorage.GetAsync<int>(key);
            await localStorage.SetAsync(key, count);
            return count;
        }
    }

    public async ValueTask SetNotificationsCountAsync(int notificationsCount) => await localStorage.SetAsync(key, notificationsCount);

    public async ValueTask IncrementNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(key))
        {
            var count = await localStorage.GetAsync<int>(key);
            count++;
            await localStorage.SetAsync(key, count);
            notificationsStateContainer.ChangeNotificationCount();
        }
    }

    public async ValueTask DecrementNotificationsCountAsync()
    {
        if (await localStorage.ExistsAsync(key))
        {
            var count = await localStorage.GetAsync<int>(key);

            if (count > 0)
            {
                count--;
                await localStorage.SetAsync(key, count);
                notificationsStateContainer.ChangeNotificationCount();
            }
        }
    }
}