using System.Security.Cryptography;

namespace eShop.Auth.Api.Utilities;

public static class CodeGenerator
{
    public static string Generate(int length)
    {
        var random = new Random();
        var max = (int)Math.Pow(10, length);
        var code = random.Next(1, max).ToString("D6");
        
        return code;
    }
}