using System.Text;
using System.Text.Json;

namespace eSystem.Core.Utilities.State;

public static class StateParser
{
    public static Dictionary<string, string> Parse(string state)
    {
        try
        {
            var bytes = Convert.FromBase64String(state);
            var dataJson = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}