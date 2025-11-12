using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Common.Network.Gateway;

public static class GatewayExtensions
{
    extension(IServiceCollection services)
    {
        public void AddGateway()
        {
            const string httpKey = "services:proxy:http:0";
            const string httpsKey = "services:proxy:https:0";
        
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var gatewayUrl = configuration.GetValue<string>(httpKey) ?? configuration.GetValue<string>(httpsKey)!;
            var options = new GatewayOptions() { Url = gatewayUrl };
        
            services.AddSingleton(options);

        }
    }
}