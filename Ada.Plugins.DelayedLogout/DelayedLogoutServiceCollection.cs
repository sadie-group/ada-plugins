using Ada.API.Interfaces.Networking.Client;
using Ada.API.Interfaces.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ada.Plugins.DelayedLogout;

public class DelayedLogoutOptions
{
    public int DelaySeconds { get; init; } = 10;
}

public class DelayedLogoutServiceCollection : IPluginServiceCollection
{
    public void Register(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<PendingLogoutRegistry>();

        serviceCollection.AddSingleton(sp => new DelayedLogoutOptions
        {
            DelaySeconds = sp.GetRequiredService<IConfiguration>()
                .GetValue("Plugins:DelayedLogout:DelaySeconds", 10)
        });
        
        serviceCollection.AddSingleton<IClientDisposalService, DelayedClientDisposalService>();
        serviceCollection.AddSingleton<IPlayerSessionResumeService, DelayedLogoutResumeService>();
    }
}
