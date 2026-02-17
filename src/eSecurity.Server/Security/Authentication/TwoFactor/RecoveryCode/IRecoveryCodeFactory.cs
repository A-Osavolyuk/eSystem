namespace eSecurity.Server.Security.Authentication.TwoFactor.RecoveryCode;

public interface IRecoveryCodeFactory
{
    public IEnumerable<string> Create(int amount = 16, int length = 10);
}