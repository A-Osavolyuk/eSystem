using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Components.Forms;

namespace eSecurity.Client.Common.Http.Extensions;

public static class HttpRequestMethodExtensions
{
    extension(HttpRequestMessage message)
    {
        public void IncludeUserAgent(HttpContext context)
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            message.Headers.Add("User-Agent", userAgent);
        }

        public void IncludeCookies(HttpContext context)
        {
            var cookies = context.Request.Cookies
                .ToDictionary(cookie => cookie.Key, cookie => cookie.Value)
                .Select(cookie => $"{cookie.Key}={cookie.Value}")
                .Aggregate((acc, item) => $"{acc}; {item}");
            
            message.Headers.Add("Cookie", cookies);
        }

        public void AddContent(HttpRequest request, HttpOptions options)
        {
            switch (options.ContentType)
            {
                case ContentTypes.Application.Json:
                {
                    if (request.Data is not null)
                    {
                        message.Content = new StringContent(JsonSerializer.Serialize(request.Data),
                            Encoding.UTF8, options.ContentType);
                    }

                    break;
                }
                case ContentTypes.Application.XwwwFormUrlEncoded:
                {
                    if (request.Data is not null)
                    {
                        var content = FormUrl.Encode(request.Data);
                        message.Content = new FormUrlEncodedContent(content);
                    }

                    break;
                }
                case ContentTypes.Multipart.FormData:
                {
                    message.Headers.Add("Accept", options.ContentType);

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
}