using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using eSystem.Core.Common.Http;
using Microsoft.AspNetCore.Components.Forms;

namespace eAccount.Common.Http.Extensions;

public static class HttpRequestMethodExtensions
{
    public static void IncludeUserAgent(this HttpRequestMessage message, HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        message.Headers.Add("User-Agent", userAgent);
    }

    public static void IncludeCookies(this HttpRequestMessage message, HttpContext context)
    {
        var cookies = context.Request.Cookies
            .ToDictionary(cookie => cookie.Key, cookie => cookie.Value)
            .Select(cookie => $"{cookie.Key}={cookie.Value}")
            .Aggregate((acc, item) => $"{acc}; {item}");
            
        message.Headers.Add("Cookie", cookies);
    }

    public static void AddContent(this HttpRequestMessage message, HttpRequest request, HttpOptions options)
    {
        switch (options.Type)
        {
            case DataType.Text:
            {
                if (request.Data is not null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(request.Data),
                        Encoding.UTF8, "application/json");
                }

                break;
            }
            case DataType.File:
            {
                message.Headers.Add("Accept", "multipart/form-data");

                var content = new MultipartFormDataContent();

                if (request.Data is not null)
                {
                    if (request.Data is IReadOnlyList<IBrowserFile> files)
                    {
                        foreach (var file in files)
                        {
                            var fileContent = new StreamContent(file.OpenReadStream());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                            content.Add(fileContent, "files", file.Name);
                        }
                    }

                    var metadata = JsonSerializer.Serialize(request.Metadata);
                    content.Add(new StringContent(metadata), "metadata");

                    message.Content = content;
                }

                break;
            }
            default: throw new NotSupportedException("Unsupported request type");
        }
    }
}