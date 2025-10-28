using System.Text;
using System.Web;

namespace eSystem.Core.Utilities.Query;

public class QueryBuilder
{
    private QueryBuilder()
    {
        QueryParams = [];
        Uri = string.Empty;
    }

    private Dictionary<string, string> QueryParams { get; set; }
    private string Uri { get; set; }

    public static QueryBuilder Create() => new QueryBuilder();

    public QueryBuilder WithUri(string uri)
    {
        Uri = uri;
        return this;
    }

    public QueryBuilder WithQueryParam(string key, string value)
    {
        var escapedValue = HttpUtility.UrlEncode(value);
        QueryParams.Add(key, escapedValue);
        return this;
    }

    public string Build()
    {
        if (QueryParams.Count == 0) return Uri;

        var builder = new StringBuilder(Uri);
        
        var isFirst = true;
        foreach (var queryParam in QueryParams)
        {
            if (isFirst)
            {
                isFirst = false;
                builder.Append($"?{queryParam.Key}={queryParam.Value}");
            }
            else
            {
                builder.Append($"&{queryParam.Key}={queryParam.Value}");
            }
        }
        
        return builder.ToString();
    }
}