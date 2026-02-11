using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenResult
{
    private TokenResult (){}

    public bool IsSucceeded { get; private set; }
    public string? Token { get; private set; }
    public Error? Error { get; private set; }
    
    public static TokenResult Success(string token) => new(){ IsSucceeded = true, Token =  token };
    public static TokenResult Fail(Error error) => new(){ IsSucceeded = false, Error = error };
}