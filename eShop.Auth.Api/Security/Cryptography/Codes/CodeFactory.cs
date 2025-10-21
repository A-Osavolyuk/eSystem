namespace eShop.Auth.Api.Security.Cryptography.Codes;

public class CodeFactory : ICodeFactory
{
    public string Create(uint length = 6)
    {
        var random = new Random();
        var max = (int)Math.Pow(10, length);
        return random.Next(1, max).ToString("D6");
    }
}