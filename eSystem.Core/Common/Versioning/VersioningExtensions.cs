using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Versioning;

public static class VersioningExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddVersioning()
        {
            builder.Services.AddApiVersioning(options =>
            {
                const string key = "api-version";
            
                options.ReportApiVersions = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader(key),
                    new HeaderApiVersionReader(key));
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = "'v'V";
            });
        }
    }
}