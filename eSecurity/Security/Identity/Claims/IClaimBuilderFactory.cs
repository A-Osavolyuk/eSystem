using eSecurity.Security.Identity.Claims.Builders;

namespace eSecurity.Security.Identity.Claims;

public interface IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder();
    public IdClaimBuilder CreateIdBuilder();
}