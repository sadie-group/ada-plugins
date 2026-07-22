using Ada.API.Interfaces.Game.Players;
using Ada.API.Interfaces.Networking.Client;
using Microsoft.Extensions.Logging;

namespace Ada.Plugins.DelayedLogout;

public class DelayedLogoutResumeService(
    ILogger<DelayedLogoutResumeService> logger,
    PendingLogoutRegistry registry) : IPlayerSessionResumeService
{
    public Task<bool> TryResumeAsync(IPlayerLogic existingPlayer, INetworkClient newClient)
    {
        if (!registry.TryTake(existingPlayer.Player.Id, out var entry) || entry == null)
        {
            return Task.FromResult(false);
        }
        
        var oldClient = entry.Client;

        oldClient.Player = null;
        oldClient.RoomUser = null;

        entry.Cts.Cancel();

        logger.LogInformation(
            "Resumed session for '{Username}' within the logout delay window",
            existingPlayer.Player.Username);

        return Task.FromResult(true);
    }
}
