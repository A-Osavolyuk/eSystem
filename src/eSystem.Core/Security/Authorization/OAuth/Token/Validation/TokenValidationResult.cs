using System.Security.Claims;

namespace eSystem.Core.Security.Authorization.OAuth.Token.Validation;

public class TokenValidationResult
{
    public bool IsValid { get; init; }
    public ClaimsPrincipal? ClaimsPrincipal { get; init; }
    
    private TokenValidationResult(){}

    public static TokenValidationResult Success(ClaimsPrincipal principal) => new TokenValidationResult
    {
        IsValid = true,
        ClaimsPrincipal = principal
    };
    
    public static TokenValidationResult Fail() => new TokenValidationResult
    {
        IsValid = false,
    };
}