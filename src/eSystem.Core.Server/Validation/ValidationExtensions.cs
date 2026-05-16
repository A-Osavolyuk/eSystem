using FluentValidation;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Server.Validation;

public static class ValidationExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddValidation<TAssemblyMarker>()
        {
            builder.Services.AddValidatorsFromAssemblyContaining<TAssemblyMarker>();
        }
    }
}