using System.Collections.Concurrent;
using Ada.API.Interfaces.Networking.Client;

namespace Ada.Plugins.DelayedLogout;

public record PendingLogout(INetworkClient Client, CancellationTokenSource Cts);

public class PendingLogoutRegistry
{
    private readonly ConcurrentDictionary<long, PendingLogout> _pending = new();

    public void Add(long playerId, PendingLogout entry)
    {
        if (_pending.TryRemove(playerId, out var previous))
        {
            previous.Cts.Cancel();
        }

        _pending[playerId] = entry;
    }

    public bool TryTake(long playerId, out PendingLogout? entry)
    {
        return _pending.TryRemove(playerId, out entry);
    }

    public void Remove(long playerId, PendingLogout entry)
    {
        ((ICollection<KeyValuePair<long, PendingLogout>>) _pending)
            .Remove(new KeyValuePair<long, PendingLogout>(playerId, entry));
    }
}
