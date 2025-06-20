using eShop.Domain.Abstraction.State;

namespace eShop.Infrastructure.State;

public class UserState : AsyncStateContainer
{
    public override async Task Change()
    {
        await StateChanged();
    }
}