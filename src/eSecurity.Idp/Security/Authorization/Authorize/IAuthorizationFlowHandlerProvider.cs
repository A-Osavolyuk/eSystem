namespace eSecurity.Idp.Security.Authorization.Authorize;

public interface IAuthorizationFlowHandlerProvider
{
    public IAuthorizationFlowHandler GetHandler(AuthorizationFlow flow);
}