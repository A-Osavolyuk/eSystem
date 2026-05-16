using eSystem.Core.Server.Security.Authentication.Schemes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace eSystem.Core.Server.Documentation.Transformers;

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
        
        document.Security?.Add(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecuritySchemeReference(AuthenticationSchemes.Basic)
                {
                    Reference = new OpenApiReferenceWithDescription()
                    {
                        Id = AuthenticationSchemes.Basic,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                []
            }
        });

        return Task.CompletedTask;
    }
}