using Ada.API;
using Ada.API.Interfaces.Game.Locale;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.API.Interfaces.Game.Locale;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.Networking.Writers.Rooms.Users;

namespace Ada.Plugins.BaseCommands.Server;

public class EnableCommand(ILocaleService localeService) : AbstractRoomChatCommand
{
    public override string Trigger => "enable";
    public override string Description => localeService["cmd.enable.describe"];
    public override List<string> Parameters { get; } = ["id"];

    public override async Task ExecuteAsync(IRoomUser user, IRoomChatCommandParameterReader reader)
    {
        if (!reader.GetInt(out var enableId))
        {
            await user.SendWhisperAsync(localeService["cmd.enable.noId"]);
            return;
        }
        
        user.ActiveEffectId = enableId;
        
        await user.Room.BroadcastDataAsync(new RoomUserEffectWriter
        {
            UserId = user.Player.Player.Id,
            EffectId = enableId,
            DelayMs = 0
        });
    }
}