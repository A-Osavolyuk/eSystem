namespace eShop.Auth.Api.Options;

public class VerificationOptions
{
    public HashSet<VerificationMethod> methods = [];
    
    public IReadOnlyCollection<VerificationMethod> Methods => methods;

    public void AddMethod(VerificationMethod method)
    {
        methods.Add(method);
    }
}