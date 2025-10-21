namespace eShop.Auth.Api.Security.TwoFactor.Recovery;

public interface IRecoveryCodeFactory
{
    public IEnumerable<string> Create(int amount = 16, int length = 10);
}