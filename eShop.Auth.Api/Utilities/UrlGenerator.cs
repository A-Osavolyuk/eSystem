namespace eShop.Auth.Api.Utilities;

public static class UrlGenerator
{
    public static string ActionLink(string action, string controller, object values, string scheme, HostString host)
    {
        var queryParams = new StringBuilder("");

        queryParams.Append("?");

        var props = values.GetType().GetProperties();

        for (int i = 0; i < props.Length; i++)
        {
            if (i == props.Length - 1)
                queryParams.Append($"{props[i].Name}={props[i].GetValue(values)}");
            else
                queryParams.Append($"{props[i].Name}={props[i].GetValue(values)}&");
        }

        return $"{scheme}://{host.Host}:{host.Port}/{controller}/{action}{queryParams}";
    }

    public static string ActionLink(string action, string hostUri, object values)
    {
        var query = new StringBuilder(hostUri).Append(action);

        query.Append("?");

        var props = values.GetType().GetProperties();

        for (int i = 0; i < props.Length; i++)
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

        for (int i = 0; i < props.Length; i++)
        {
            if (i == props.Length - 1)
                query.Append($"{props[i].Name}={props[i].GetValue(values)}");
            else
                query.Append($"{props[i].Name}={props[i].GetValue(values)}&");
        }

        return query.ToString();
    }
}