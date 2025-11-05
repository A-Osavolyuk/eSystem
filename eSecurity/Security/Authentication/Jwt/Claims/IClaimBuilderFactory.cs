namespace eSecurity.Security.Authentication.Jwt.Claims;

public interface IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder();
    public IdClaimBuilder CreateIdBuilder();
}