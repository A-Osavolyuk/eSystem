using System.Text;
using System.Web;
using eSystem.Core.Enums;

namespace eSystem.Core.Utilities.Query;

public class QueryBuilder
{
    private QueryBuilder()
    {
        QueryParams = new Dictionary<string, string>();
        BaseUri = string.Empty;
    }

    private Dictionary<string, string> QueryParams { get; }
    private string BaseUri { get; set; }

    public static QueryBuilder Create() => new();

    public QueryBuilder WithUri(string uri)
    {
        BaseUri = uri;
        return this;
    }

    public QueryBuilder WithQueryParam(string key, string value)
    {
        QueryParams[key] = HttpUtility.UrlEncode(value);
        return this;
    }
    
    public QueryBuilder WithQueryParam(string key, bool value)
    {
        QueryParams[key] = HttpUtility.UrlEncode(value.ToString());
        return this;
    }
    
    public QueryBuilder WithQueryParam(string key, Guid value)
    {
        QueryParams[key] = HttpUtility.UrlEncode(value.ToString());
        return this;
    }

    public QueryBuilder WithQueryParam<TEnum>(string key, TEnum value) where TEnum : struct, Enum
    {
        var stringValue = value.GetString();
        QueryParams[key] = HttpUtility.UrlEncode(stringValue);
        return this;
    }

    public string Build()
    {
        if (QueryParams.Count == 0)
            return BaseUri;

        var builder = new StringBuilder(BaseUri);
        var separator = BaseUri.Contains('?') ? '&' : '?';

        foreach (var (key, value) in QueryParams)
        {
            builder.Append($"{separator}{key}={value}");
            separator = '&';
        }

        return builder.ToString();
    }
}