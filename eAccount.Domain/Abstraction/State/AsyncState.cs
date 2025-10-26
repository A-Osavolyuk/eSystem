namespace eAccount.Domain.Abstraction.State;

public abstract class AsyncState
{
    public event Func<Task>? OnChange;

    public abstract Task Change();

    protected async Task StateChanged() => await OnChange!.Invoke();
}