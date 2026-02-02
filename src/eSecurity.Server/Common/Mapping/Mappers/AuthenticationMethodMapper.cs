using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Common.Mapping.Mappers;

public sealed class AuthenticationMethodMapper : IMapper<LinkedAccountType, string>
{
    public string Map(LinkedAccountType input)
    {
        return input switch
        {
            LinkedAccountType.Google => AuthenticationMethods.OAuth.Google,
            LinkedAccountType.Facebook => AuthenticationMethods.OAuth.Facebook,
            LinkedAccountType.Microsoft => AuthenticationMethods.OAuth.Microsoft,
            LinkedAccountType.X => AuthenticationMethods.OAuth.X,
            _ => throw new NotSupportedException()
        };
    }
}