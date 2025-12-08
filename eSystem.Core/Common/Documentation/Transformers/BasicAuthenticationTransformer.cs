using eSystem.Core.Security.Authentication.Constants;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace eSystem.Core.Common.Documentation.Transformers;

public class BasicAuthenticationTransformer : IOpenApiDocumentTransformer
{

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes?.Add(BasicAuthenticationDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = BasicAuthenticationDefaults.AuthenticationScheme.ToLower(),
                Description = "Enter your Basic Authentication value in the format: `Basic {value}`"
            });

        document.SecurityRequirements?.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = BasicAuthenticationDefaults.AuthenticationScheme
                    }
                },[]
            }
        });

        return Task.CompletedTask;
    }
}