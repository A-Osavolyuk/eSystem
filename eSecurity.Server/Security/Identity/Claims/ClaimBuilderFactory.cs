using eSecurity.Server.Security.Identity.Claims.Builders;

namespace eSecurity.Server.Security.Identity.Claims;

public class ClaimBuilderFactory : IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder() => AccessClaimBuilder.Create();
    public IdClaimBuilder CreateIdBuilder() => IdClaimBuilder.Create();
}