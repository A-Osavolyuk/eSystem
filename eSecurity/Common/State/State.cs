namespace eSecurity.Common.State;

public abstract class State
{
    public event Func<Task>? OnChange;

    public abstract Task Change();

    protected async Task StateChanged() => await OnChange!.Invoke();
}