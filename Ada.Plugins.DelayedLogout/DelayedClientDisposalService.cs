using Ada.API.Interfaces.Networking.Client;
using Microsoft.Extensions.Logging;

namespace Ada.Plugins.DelayedLogout;

public class DelayedClientDisposalService(
    ILogger<DelayedClientDisposalService> logger,
    INetworkClientRepository clientRepository,
    PendingLogoutRegistry registry,
    DelayedLogoutOptions options) : IClientDisposalService
{
    public async Task HandleDisconnectAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player == null || options.DelaySeconds <= 0)
        {
            await clientRepository.TryRemoveAsync(client.Guid);
            await client.DisposeAsync();
            return;
        }

        var playerId = player.Player.Id;
        var entry = new PendingLogout(client, new CancellationTokenSource());

        registry.Add(playerId, entry);

        logger.LogInformation(
            "Delaying logout of '{Username}' by {Delay}s",
            player.Player.Username,
            options.DelaySeconds);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(options.DelaySeconds), entry.Cts.Token);
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            registry.Remove(playerId, entry);
        }

        await clientRepository.TryRemoveAsync(client.Guid);
        await client.DisposeAsync();
    }
}
