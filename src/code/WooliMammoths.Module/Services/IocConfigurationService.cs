using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace WooliMammoths.Module.Services
{
    public class IocConfigurationService : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllersInCurrentAssembly();
            // serviceCollection.AddTransient()
        }
    }
}