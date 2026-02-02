using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Common.Mapping.Mappers;

namespace eSecurity.Server.Common.Mapping;

public static class MappingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMapping()
        {
            builder.Services.AddSingleton<IMappingProvider, MappingProvider>();
            builder.Services.AddTransient<IMapper<LinkedAccountType, string>, AuthenticationMethodMapper>();
        }
    }
}