namespace eShop.Domain.Abstraction.State;

public abstract class AsyncStateContainer
{
    public event Func<Task>? OnChange;

    public abstract Task Change();

    protected async Task StateChanged() => await OnChange!.Invoke();
}