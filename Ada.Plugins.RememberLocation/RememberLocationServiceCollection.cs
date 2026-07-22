using Ada.API.Interfaces.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Ada.Plugins.RememberLocation;

public class RememberLocationServiceCollection : IPluginServiceCollection
{
    public void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<LastLocationStore>();
    }
}
