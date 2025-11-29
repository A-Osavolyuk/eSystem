namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public interface IIntrospectionResolver
{
    public IIntrospectionStrategy Resolve(IntrospectionType type);
}