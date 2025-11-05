namespace eSecurity.Security.Authentication.JWT.Claims;

public interface IClaimBuilderFactory
{
    public AccessClaimBuilder CreateAccessBuilder();
    public IdClaimBuilder CreateIdBuilder();
}