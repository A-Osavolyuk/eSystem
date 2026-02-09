using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Common.Mapping.Mappers;

public sealed class AuthenticationMethodMapper : IMapper<LinkedAccountType, string>
{
    public string Map(LinkedAccountType input)
    {
        return input switch
        {
            LinkedAccountType.Google => AuthenticationMethods.Google,
            LinkedAccountType.Facebook => AuthenticationMethods.Facebook,
            LinkedAccountType.Microsoft => AuthenticationMethods.Microsoft,
            LinkedAccountType.X => AuthenticationMethods.X,
            _ => throw new NotSupportedException()
        };
    }
}