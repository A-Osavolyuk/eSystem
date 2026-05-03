using eSecurity.Core.Common.DTOs;
using eSystem.Core.Primitives;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Token;

public sealed class ValidationResult
{
    public bool Succeeded { get; set; }
    public Error? Error { get; set; }
    public List<ClaimValue> Claims { get; set; } = [];
    
    private ValidationResult() {}

    public static ValidationResult Success(List<ClaimValue> claims) => new() { Succeeded = true, Claims = claims };
    public static ValidationResult Failure(Error error) => new() { Succeeded = false, Error = error };

    public Error GetError() => Error ?? throw new Exception("Error is null");
}