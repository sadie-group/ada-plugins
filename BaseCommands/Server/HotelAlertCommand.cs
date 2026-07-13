using Ada.API;
using Ada.API.Interfaces.Game.Locale;
using Ada.API.Interfaces.Game.Players;
using Ada.API.Interfaces.Game.Rooms.Chat.Commands;
using Ada.API.Interfaces.Game.Rooms.Users;
using Ada.Networking.Writers.Players;

namespace Ada.Plugins.BaseCommands.Server;

public class HotelAlertCommand(IPlayerRepository playerRepository,
    ILocaleService localeService) : AbstractRoomChatCommand
{
    public override string Trigger => "ha";
    public override string Description => localeService["cmd.ha.describe"];
    
    public override async Task ExecuteAsync(IRoomUser user, IRoomChatCommandParameterReader reader)
    {
        if (!reader.GetSentence(out var message) || string.IsNullOrWhiteSpace(message) || message.Length < 5)
        {
            await user.SendWhisperAsync(localeService["cmd.ha.badMessage"]);
            return;
        }
        
        var author = user.Player.Player.Username;
        
        await playerRepository.BroadcastDataAsync(
            new PlayerAlertWriter
            {
                Message = $"{message}\n\n- {author} at {DateTime.Now:HH:mm}"
            });
    }
    
    public override List<string> PermissionsRequired{ get; set; } = ["command_hotel_alert"];
    public override List<string> Parameters { get; } = ["message"];
}