using eShop.Infrastructure.State;

namespace eShop.Infrastructure.Services;

public class NotificationService(
    ILocalStorageService storageService, 
    IStorage localStorage,
    NotificationsStateContainer notificationsStateContainer) : INotificationService
{
    private readonly ILocalStorageService storageService = storageService;
    private readonly IStorage localStorage = localStorage;
    private readonly NotificationsStateContainer notificationsStateContainer = notificationsStateContainer;
    private readonly string key = "notifications-count";

    public async ValueTask<int> GetNotificationsCountAsync()
    {
        if (await storageService.ContainKeyAsync(key))
        {
            return await storageService.GetItemAsync<int>(key);
        }
        else
        {
            var count = await localStorage.GetAsync<int>(key);
            await storageService.SetItemAsync(key, count);
            return count;
        }
    }

    public async ValueTask SetNotificationsCountAsync(int notificationsCount) => await storageService.SetItemAsync(key, notificationsCount);

    public async ValueTask IncrementNotificationsCountAsync()
    {
        if (await storageService.ContainKeyAsync(key))
        {
            var count = await storageService.GetItemAsync<int>(key);
            count++;
            await storageService.SetItemAsync(key, count);
            notificationsStateContainer.ChangeNotificationCount();
        }
    }

    public async ValueTask DecrementNotificationsCountAsync()
    {
        if (await storageService.ContainKeyAsync(key))
        {
            var count = await storageService.GetItemAsync<int>(key);

            if (count > 0)
            {
                count--;
                await storageService.SetItemAsync(key, count);
                notificationsStateContainer.ChangeNotificationCount();
            }
        }
    }
}