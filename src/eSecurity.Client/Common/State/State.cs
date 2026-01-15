namespace eSecurity.Client.Common.State;

public abstract class State
{
    public event Func<Task>? OnChange;

    public virtual async Task Change()
    {
        await StateChanged();
    }

    protected async Task StateChanged() => await OnChange!.Invoke();
}