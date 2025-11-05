namespace eSecurity.Security.Authentication.Jwt.Claims;

public class ClaimBuilderFactory : IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder() => AccessClaimBuilder.Create();
    public IdClaimBuilder CreateIdBuilder() => IdClaimBuilder.Create();
}