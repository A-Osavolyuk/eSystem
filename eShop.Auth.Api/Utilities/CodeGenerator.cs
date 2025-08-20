namespace eShop.Auth.Api.Utilities;

public static class CodeGenerator
{
    public static string Generate(byte length)
    {
        var code = GenerateCode(length);
        return code;
    }

    public static List<string> GenerateMany(byte length, byte amount = 10)
    {
        var codes = new List<string>();
        for (var i = 0; i < amount; i++)
        {
            var code = GenerateCode(length);
            codes.Add(code);
        }
        
        return codes;
    }

    private static string GenerateCode(byte length)
    {
        var random = new Random();
        var max = (int)Math.Pow(10, length);
        var code = random.Next(1, max).ToString("D6");
        
        return code;
    }
}