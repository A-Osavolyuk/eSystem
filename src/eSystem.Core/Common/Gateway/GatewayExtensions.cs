using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Common.Gateway;

public static class GatewayExtensions
{
    extension(IServiceCollection services)
    {
        public void AddGateway()
        {
            const string httpKey = "PROXY_HTTP";
            const string httpsKey = "PROXY_HTTPS";
        
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var gatewayUrl = configuration.GetValue<string>(httpKey) ?? configuration.GetValue<string>(httpsKey)!;
            var options = new GatewayOptions { Url = gatewayUrl };
        
            services.AddSingleton(options);

        }
    }
}