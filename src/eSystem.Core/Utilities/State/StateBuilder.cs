using System.Text;
using System.Text.Json;

namespace eSystem.Core.Utilities.State;

public class StateBuilder
{
    private StateBuilder()
    {
    }
    
    private Dictionary<string, string> _data = [];
    
    public static StateBuilder Create() => new();
    
    public StateBuilder WithData(string key, string value)
    {
        _data[key] = value;
        return this;
    }
    
    public StateBuilder WithData(string key, bool value)
    {
        _data[key] = value.ToString();
        return this;
    }

    public string Build()
    {
        var dataJson = JsonSerializer.Serialize(_data);
        var bytes = Encoding.UTF8.GetBytes(dataJson);
        return Convert.ToBase64String(bytes);
    }
}