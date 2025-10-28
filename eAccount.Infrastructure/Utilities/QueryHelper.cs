namespace eAccount.Infrastructure.Utilities;

public static class QueryHelper
{
    public static Dictionary<string, string> GetQueryParameters(string uri)
    {
        if (!uri.Contains('?')) return [];
        
        return uri
            .Split('?')
            .Last()
            .Split('&')
            .Select(x => x.Split('='))
            .ToDictionary(x => x.First(), x => x.Last());
    }
}