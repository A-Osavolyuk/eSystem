namespace eSecurity.Security.Authentication.JWT.Claims;

public class ClaimBuilderFactory : IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder() => AccessClaimBuilder.Create();
    public IdClaimBuilder CreateIdBuilder() => IdClaimBuilder.Create();
}