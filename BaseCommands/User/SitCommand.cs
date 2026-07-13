using Ada.API;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.Core.Enums.Game.Rooms.Users;

namespace Ada.Plugins.BaseCommands.User;

public class SitCommand : AbstractRoomChatCommand
{
    public override string Trigger => "sit";
    public override string Description => "Makes your avatar sit down";
    
    public override Task ExecuteAsync(IRoomUser user, IRoomChatCommandParameterReader reader)
    {
        if (user.StatusMap.ContainsKey(RoomUserStatus.Sit))
        {
            return Task.CompletedTask;
        }
        
        user.AddStatus(RoomUserStatus.Sit, "0.5");
        return Task.CompletedTask;
    }
}