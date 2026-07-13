using Ada.API;
using Ada.API.Interfaces.Game.Locale;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.Core.Enums.Miscellaneous;
using Ada.Networking.Writers.Rooms.Users;

namespace Ada.Plugins.BaseCommands.User;

public class MoonWalkCommand(ILocaleService localeService) : AbstractRoomChatCommand
{
    public override string Trigger => "moonwalk";
    public override string Description => localeService["cmd.moonWalk.describe"];
    
    public override async Task ExecuteAsync(IRoomUser user, IRoomChatCommandParameterReader reader)
    {
        user.MoonWalking = !user.MoonWalking;
        
        var effectId = user.MoonWalking ? (int) EffectIds.Moonwalk : 0;
        
        await user.Room.BroadcastDataAsync(new RoomUserEffectWriter
        {
            UserId = user.Player.Player.Id,
            EffectId = effectId,
            DelayMs = 0
        });
    }
}