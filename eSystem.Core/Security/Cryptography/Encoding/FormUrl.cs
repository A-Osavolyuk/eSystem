using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Security.Cryptography.Encoding;

public static class FormUrl
{
    public static Dictionary<string, string> Encode(object obj)
    {
        var dict = new Dictionary<string, string>();

        foreach (var prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj)?.ToString();
            if (string.IsNullOrEmpty(value)) continue;
            
            var fromFormAttr = prop
                .GetCustomAttributes(typeof(FromFormAttribute), false)
                .FirstOrDefault() as FromFormAttribute;
            
            var key = fromFormAttr?.Name ?? prop.Name;
            dict[key] = value;
        }

        return dict;
    }
}