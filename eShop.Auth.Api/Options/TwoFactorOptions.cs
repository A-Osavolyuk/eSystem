namespace eShop.Auth.Api.Options;

public class TwoFactorOptions
{
    public HashSet<TwoFactorMethod> methods = [];
    
    public IReadOnlyCollection<TwoFactorMethod> Methods => methods;

    public void AddMethod(TwoFactorMethod method)
    {
        methods.Add(method);
    }
}