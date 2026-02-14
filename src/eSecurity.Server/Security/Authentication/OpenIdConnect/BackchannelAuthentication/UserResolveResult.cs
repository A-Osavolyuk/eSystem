using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class UserResolveResult
{
    public bool Succeeded { get; private set; }
    public UserEntity? User { get; private set; }
    private Error? Error { get; set; }

    private UserResolveResult()
    {
        
    }
    
    public static UserResolveResult Success(UserEntity user) => new() { Succeeded = true, User =  user };
    public static UserResolveResult Fail(Error error) => new(){ Succeeded = false, Error = error };

    public Error GetError() => Error!;
}