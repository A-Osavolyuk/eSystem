namespace eSecurity.Idp.Security.Cryptography;

public static class CodeFactory
{
    public static string Create(uint length = 6)
    {
        var random = new Random();
        var max = (int)Math.Pow(10, length);
        return random.Next(1, max).ToString($"D{length}");
    }
}