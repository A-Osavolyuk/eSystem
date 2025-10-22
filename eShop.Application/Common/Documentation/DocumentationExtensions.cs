using eShop.Application.Common.Documentation.Transformers;
using Microsoft.Extensions.Hosting;

namespace eShop.Application.Common.Documentation;

public static class DocumentationExtensions
{
    public static void AddDocumentation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerTokenTransformer>();
        });
    }
}