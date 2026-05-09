using System.Text;
using System.Text.Json;
using eSystem.Core.Form;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Cryptography.Encoding;

namespace eSystem.Core.Http.Extensions;

public static class HttpRequestMessageExtensions
{
    extension(HttpRequestMessage message)
    {
        public void AddContent(ApiRequest request, ApiOptions options)
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
                        var content = request.Data switch
                        {
                            IFormRequest formRequest => formRequest.GetForm(),
                            _ => FormUrl.Encode(request.Data)
                        };
                        
                        message.Content = new FormUrlEncodedContent(content);
                    }

                    break;
                }
                default: throw new NotSupportedException("Unsupported request type");
            }
        }
    }
}