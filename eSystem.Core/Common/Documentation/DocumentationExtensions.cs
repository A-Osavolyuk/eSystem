using eSystem.Core.Common.Documentation.Transformers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Documentation;

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