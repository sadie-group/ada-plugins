using Ada.API;
using Ada.API.Interfaces.Game.Locale;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.Networking.Writers.Rooms.Users;

namespace Ada.Plugins.BaseCommands.User;

public class IdleCommand(ILocaleService localeService) : AbstractRoomChatCommand
{
    public override string Trigger => "idle";
    public override string Description => localeService["cmd.idle.describe"];
    
    public override async Task ExecuteAsync(IRoomUser user, IRoomChatCommandParameterReader reader)
    {
        await user.Room.BroadcastDataAsync(new RoomUserIdleWriter
        {
            UserId = user.Player.Player.Id,
            IsIdle = true
        });
    }
}