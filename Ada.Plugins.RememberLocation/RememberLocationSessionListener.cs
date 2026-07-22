using System.Drawing;
using Ada.API;
using Ada.API.Interfaces.Game.Players;
using Ada.API.Interfaces.Game.Rooms;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.API.Interfaces.Networking.Client;
using Ada.API.Interfaces.Plugins;
using Ada.Core.Enums.Miscellaneous;
using Ada.Networking.Writers.Rooms.Users;

namespace Ada.Plugins.RememberLocation;

public class RememberLocationSessionListener(
    LastLocationStore store,
    IRoomRepository roomRepository) : IPlayerSessionListener
{
    public async Task OnLoginAsync(INetworkClient client, IPlayerLogic player, bool resumed)
    {
        var location = ResolveLocation(player, resumed);

        if (location == null)
        {
            return;
        }

        player.State.RoomEntryOverride = new PlayerRoomEntryOverride(
            location.RoomId,
            new Point(location.X, location.Y),
            (HDirection) location.Direction);

        await client.WriteToStreamAsync(new RoomForwardEntryWriter
        {
            RoomId = location.RoomId
        });
    }

    public async Task OnDisconnectedAsync(IPlayerLogic player, IRoomUser? roomUser)
    {
        if (roomUser == null)
        {
            return;
        }

        store.Set(player.Player.Id, new LastLocation(
            (int) roomUser.Room.Room.Id,
            roomUser.Point.X,
            roomUser.Point.Y,
            (int) roomUser.Direction));

        await store.SaveAsync();
    }

    private LastLocation? ResolveLocation(IPlayerLogic player, bool resumed)
    {
        if (resumed && player.State.CurrentRoomId != 0)
        {
            var room = roomRepository.TryGetRoomById(player.State.CurrentRoomId);

            if (room != null &&
                room.UserRepository.TryGetById(player.Player.Id, out var roomUser) &&
                roomUser != null)
            {
                return new LastLocation(
                    player.State.CurrentRoomId,
                    roomUser.Point.X,
                    roomUser.Point.Y,
                    (int) roomUser.Direction);
            }
        }

        return store.TryGet(player.Player.Id, out var location) ? location : null;
    }
}
