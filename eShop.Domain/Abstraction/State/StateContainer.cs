namespace eShop.Domain.Abstraction.State;

public abstract class StateContainer
{
    public event Action? OnChange;

    public void Change()
    {
        StateChanged();
    }

    private void StateChanged() => OnChange!.Invoke();
}