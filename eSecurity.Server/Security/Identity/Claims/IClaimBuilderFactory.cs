using eSecurity.Server.Security.Identity.Claims.Builders;

namespace eSecurity.Server.Security.Identity.Claims;

public interface IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder();
    public IdClaimBuilder CreateIdBuilder();
}