namespace eShop.Auth.Api.Utilities;

public static class UrlGenerator
{
    public static string ActionLink(string uri, object values)
    {
        var query = new StringBuilder(uri);

        query.Append("?");

        var props = values.GetType().GetProperties();

        for (var i = 0; i < props.Length; i++)
        {
            if (i == props.Length - 1)
                query.Append($"{props[i].Name}={props[i].GetValue(values)}");
            else
                query.Append($"{props[i].Name}={props[i].GetValue(values)}&");
        }

        return query.ToString();
    }

    public static string Action(string action, string controller, object values)
    {
        var query = new StringBuilder($"/api/v1/{controller}/{action}");

        query.Append("?");

        var props = values.GetType().GetProperties();

        for (var i = 0; i < props.Length; i++)
        {
            if (i == props.Length - 1)
                query.Append($"{props[i].Name}={props[i].GetValue(values)}");
            else
                query.Append($"{props[i].Name}={props[i].GetValue(values)}&");
        }

        return query.ToString();
    }
}