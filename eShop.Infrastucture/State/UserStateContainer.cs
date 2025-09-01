using eShop.Domain.Abstraction.State;

namespace eShop.Infrastructure.State;

public class UserStateContainer : AsyncStateContainer
{
    public override async Task Change()
    {
        await StateChanged();
    }
}