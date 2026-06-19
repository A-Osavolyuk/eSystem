namespace eSecurity.Idp.Common.Validation;

public static class Normalizer
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        return value.ToUpperInvariant();
    }
}