namespace eSecurity.Server.Common.Mapping;

public static class MappingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMapping()
        {
            builder.Services.AddSingleton<IMappingProvider, MappingProvider>();
        }
    }
}