using eShop.Domain.Abstraction.State;

namespace eShop.BlazorWebUI.State;

public class UserState : AsyncStateContainer
{
    public override async Task Change()
    {
        await StateChanged();
    }
}