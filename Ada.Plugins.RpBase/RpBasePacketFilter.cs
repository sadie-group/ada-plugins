using Ada.API.Interfaces.Networking.Client;
using Ada.API.Interfaces.Networking.Events.Filters;
using Ada.API.Interfaces.Networking.Events.Handlers;
using Ada.Core.Enums.Game.Players;

namespace Ada.Plugins.RpBase;

public class RpBasePacketFilter : INetworkPacketEventFilter
{
    private static readonly HashSet<string> BlockedHandlers =
    [
        "PlayerChangedMottoEventHandler"
    ];

    private static readonly HashSet<string> StaffOnlyHandlers =
    [
        "NavigatorSearchEventHandler",
        "PlayerStalkEventHandler",
        "PlayerCreateRoomEventHandler",
        "RoomUserGoToHotelViewEventHandler",
        "PlayerSetHomeRoomEventHandler"
    ];

    public Task<bool> AllowAsync(INetworkClient client, INetworkPacketEventHandler eventHandler)
    {
        var handlerName = eventHandler.GetType().Name;

        if (BlockedHandlers.Contains(handlerName))
        {
            return Task.FromResult(false);
        }

        if (StaffOnlyHandlers.Contains(handlerName))
        {
            var isStaff = client.Player?.HasPermission(PlayerPermissionName.Moderator) ?? false;

            return Task.FromResult(isStaff);
        }

        return Task.FromResult(true);
    }
}
