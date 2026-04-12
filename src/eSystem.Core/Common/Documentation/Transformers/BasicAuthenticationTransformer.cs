using eSystem.Core.Security.Authentication.Schemes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace eSystem.Core.Common.Documentation.Transformers;

public class BasicAuthenticationTransformer : IOpenApiDocumentTransformer
{

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes?.Add(AuthenticationSchemes.Basic,
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = AuthenticationSchemes.Basic.ToLower(),
                Description = "Enter your Basic Authentication value in the format: `Basic {value}`"
            });

        document.SecurityRequirements?.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = AuthenticationSchemes.Basic
                    }
                },[]
            }
        });

        return Task.CompletedTask;
    }
}