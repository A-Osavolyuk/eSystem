using eSecurity.Security.Identity.Claims.Builders;

namespace eSecurity.Security.Identity.Claims;

public class ClaimBuilderFactory : IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder() => AccessClaimBuilder.Create();
    public IdClaimBuilder CreateIdBuilder() => IdClaimBuilder.Create();
}